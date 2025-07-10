import Constants from "@/Constants.js";
import Row from "../Row/Row.vue"
import { processDisplayMessage } from "@/services/terminal_services.js";
import {gql} from "@apollo/client/core";

export default {
    name: 'Terminal',
    components: {
      Row
    },

    data() {
        return {
            sessionKey: null,
            rows: [],
        }
    },

    async mounted() {
        const result = await this.$apollo.mutate({
            mutation: gql(`mutation Connect($address: String!, $port: Int!) {
                connect(connectRequest:  {
                    address: $address
                    port: $port
                }) {
                    sessionKey
                    address
                    port
                }
            }`),
            variables: { address: this.$route.query.address, port: parseInt(this.$route.query.port) }
        });

        this.sessionKey = result.data.connect.sessionKey;
    },

    computed: {
        Constants() {
            return Constants
        },
    },

    apollo: {
            display: {
                query: gql`query($sessionKey: String!) {
                    display(sessionKey: $sessionKey) {
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
                }`,
                subscribeToMore: {
                    document: gql`subscription display($sessionKey: String!) {
                        display(sessionKey: $sessionKey) {
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
                    }`,
                    variables() {
                        return {sessionKey: this.sessionKey}
                    },
                    skip() {
                        return !this.sessionKey
                    },
                    updateQuery(previousResult, { subscriptionData: { data: { display: { fieldData }}}})  {
                        this.rows = processDisplayMessage(fieldData)
                    }
                },
                variables() {
                    return {sessionKey: this.sessionKey}
                },
                skip() {
                    return !this.sessionKey
                },
                result({data: { display: { fieldData }}}) {
                    this.rows = processDisplayMessage(fieldData)
                }
            },
    },
    
    methods: {

        functionKey() {
            this.$apollo.mutate({
                mutation: gql`mutation SubmitFields($submission: Submission!) {
                    submitFields(submission: $submission) {
                        code
                    }
                }`,
                variables: {
                    submission: {
                        sessionKey: this.sessionKey,
                        fieldSubmission: {
                            aid: "CLEAR",
                            cursorRow: 17,
                            cursorCol: 22,
                            fieldData: [{
                                row: 22,
                                col: 11,
                                value: "HERC01",
                                address: 1771
                            }]
                        }
                    }
                }
            });
        },

        inputChanged(row, col, value) {
            console.info(row, col, value);
        },

        focusChanged(row, col) {
            console.info(row, col)
        },

        setRows(rows) {
            this.rows = rows;
        }
    }
}
