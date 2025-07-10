import { ApolloClient, gql, InMemoryCache } from "@apollo/client/core";

export default {
    data() {
        return {
            connection: {},
            address: '127.0.0.1',
            port: '3270'
        }
    },
    apollo: {

    },
    methods: {
        async login() {
            this.$router.push({name: 'terminal', query: { address: this.address, port: this.port }});
        }
    }
}
