import { createRouter , createWebHistory } from "vue-router";

import Connect from './components/Connect/Connect.vue'
import Terminal from './components/Terminal/Terminal.vue'

const routes = [
    { path: '/', component: Connect },
    { path: '/connect', name: 'connect', component: Connect },
    { path: '/terminal', name: 'terminal', component: Terminal }
]

const router = createRouter({
    history: createWebHistory(),
    routes
})

export default router;
