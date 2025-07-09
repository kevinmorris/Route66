import { createRouter , createWebHistory } from "vue-router";

import Connect from './components/Connect/Connect.vue'
import Terminal from './components/Terminal/Terminal.vue'

const routes = [
    { path: '/', component: Terminal },
    { path: '/connect', component: Connect },
    { path: '/terminal', component: Terminal }
]

const router = createRouter({
    history: createWebHistory(),
    routes
})

export default router;
