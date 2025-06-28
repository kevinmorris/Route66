import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { createPinia } from 'pinia';
import {ApolloClient, HttpLink, InMemoryCache, split} from "@apollo/client/core";
import {createApolloProvider} from "@vue/apollo-option";
import {getMainDefinition} from "@apollo/client/utilities";
import {GraphQLWsLink} from "@apollo/client/link/subscriptions";
import {createClient} from "graphql-ws";

const wsLink = new GraphQLWsLink(createClient({
    url: 'ws://127.0.0.1:7149/graphql'
}));

const httpLink = new HttpLink({
    uri: 'http://127.0.0.1:7149/graphql'
})

const link = split(
    ({ query }) => {
        const def = getMainDefinition(query);
        return def.kind === 'OperationDefinition' && def.operation === 'subscription';
    },
    wsLink,
    httpLink
);

const apolloClient = new ApolloClient({
    link: link,
    cache: new InMemoryCache()
});

const apolloProvider = createApolloProvider({
    defaultClient: apolloClient
})

const app = createApp(App);
app.use(router)
app.use(createPinia());
app.use(apolloProvider);
app.mount('#app')
