import { defineStore } from 'pinia'

export const useConnectionStore = defineStore('connection', {

    state: () => {
        return {
            url: '',
            port: '',
            sessionKey: ''
        }
    },
    actions: {
        login(url, port, sessionKey) {
            this.url = url;
            this.port = port;
            this.sessionKey = sessionKey;
        },
        logout() {
            this.$reset();
        },

    }
});
