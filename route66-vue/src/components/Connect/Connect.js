import {ApolloClient, gql, InMemoryCache} from "@apollo/client/core";

export default {
    data() {
        return {
            connection: {},
            url: '127.0.0.1',
            port: '3270'
        }
    },
    apollo: {

    },
    methods: {
        async login() {
            const connection = await this.$apollo.mutate({
                mutation: gql(`mutation Connect {
                  connect(connectParams:  {
                     address: "asdfasdf"
                     port: 34
                  }) {
                    sessionKey
                    address
                    port
                  }
                }`)
            });

            console.info(connection);

            const display = await this.$apollo.query({
                query: gql(`{
                  display {
                    fieldData {
                      row
                      col
                      value
                      isProtected
                      address
                      length
                      dirty
                      cursor
                    }
                  }
                }`)
            });

            console.info(display)
        }
    }
}
