using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Route66Blazor.Models;
using Services;
using Services.Models;
using Services.Translators;
using Util;

namespace Route66Blazor.Components.Pages
{
    public partial class TerminalDisplay
    {
        protected ITN3270Service<XElement> NetworkService { get; private set;  }

        [Inject]
        protected ProtectedSessionStorage SessionStorage { get; init; }

        [Inject]
        protected IMemoryCache Cache { get; init; }

        [Inject]
        protected ILogger<TerminalDisplay> Logger { get; init; }

        [SupplyParameterFromQuery]
        public string Address { get; set; }

        [SupplyParameterFromQuery]
        public int Port { get; set; }

        private ElementReference _container;
        private Grid _grid;
        private (int, int) _cursor = (-1, -1);
        private readonly Action<(int, int)> _cursorAction;

        public TerminalDisplay()
        {
            _cursorAction = OnFocusChanged;
        }

        private void OnFocusChanged((int, int) coords)
        {
            _cursor = coords;
        }

        private async Task KeyDown(KeyboardEventArgs args)
        {
            if (args.Code == "Enter")
            {
                await SendUserData(AID.ENTER);
            }
        }

        private async Task Reset(MouseEventArgs args)
        {
            await Task.CompletedTask;
        }

        private async Task FunctionKey(byte key)
        {
            await NetworkService.SendKeyAsync(key);
        }

        private async Task SendUserData(byte aid)
        {
            var fields = _grid.FieldData.SelectMany(list => list).ToList();
            var inputFields = fields
                .Where(f => f is { IsProtected: false, Dirty: true })
                .ToList();
            
            var cursorField = inputFields.FirstOrDefault(f => f.Row == _cursor.Item1 && f.Col == _cursor.Item2) ??
                              inputFields.LastOrDefault() ??
                              fields.Last();
            
            var cursor = (_cursor.Item1, _cursor.Item2 + cursorField.Value.Length);
            
            await NetworkService.SendFieldsAsync(
                aid,
                cursor.Item1,
                cursor.Item2,
                inputFields);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!firstRender)
            {
                return;
            }

            var serviceIdResult = await SessionStorage.GetAsync<string>(Constants.KEY_TN3270SERVICE_SESSION_STORAGE);

            var serviceId = serviceIdResult is { Success: true, Value: not null }
                ? serviceIdResult.Value
                : await SessionStorage.Let(async s =>
                {
                    var id = Guid.NewGuid().ToString();
                    await s.SetAsync(Constants.KEY_TN3270SERVICE_SESSION_STORAGE, id);
                    return id;
                });

            NetworkService = FetchService(serviceId) ;
            _grid.Handler = NetworkService.Handler;

            await _container.FocusAsync(true);
            NetworkService.Update(true);
        }

        private ITN3270Service<XElement> FetchService(string serviceId)
        {
            if (!Cache.TryGetValue(serviceId, out ITN3270Service<XElement>? tn3270Service))
            {
                tn3270Service = new TN3270Service<XElement>(new Xml3270Translator());
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30));

                Cache.Set(serviceId, tn3270Service, cacheEntryOptions);
            }

            return tn3270Service is not null
                ? tn3270Service.Also(service => service.Connect(Address, Port))
                : throw new InvalidOperationException("ITN3270Service creation or retrieval failed.");
        }
    }
}
