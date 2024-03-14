import { Routes } from '@angular/router';
import { ConnectComponent } from "./connect/connect.component";
import { TerminalComponent } from "./terminal/terminal.component";

export const routes: Routes = [

  { path: 'connect', component: ConnectComponent },
  { path: 'terminal', component: TerminalComponent },
];
