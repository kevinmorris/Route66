import Constants from "@/Constants.js";
import Row from "../Row/Row.vue"
import {
    createFieldSubmission,
    gqlConstants,
    inputValueChanged,
    processDisplayMessage
} from "@/services/terminal_services.js";
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
            cursor: [-1, -1],
            errorMessage: null
        }
    },

    mounted() {
        window.addEventListener('keydown', this.handleKeyDown)

        this.$apollo.mutate({
            mutation: gqlConstants.connect,
            variables: {
                address: this.$route.query.address,
                port: parseInt(this.$route.query.port)
            },
        }).then((value) => {
            this.errorMessage = null;
            this.sessionKey = value.data.connect.sessionKey;
        }).catch((error) => {
            this.errorMessage = error
        });
    },

    computed: {
        Constants() {
            return Constants
        },
    },

    apollo: {
        display: {
            query: gqlConstants.displayQuery,
            subscribeToMore: {
                document: gqlConstants.displaySubscription,
                variables() {
                    return {sessionKey: this.sessionKey}
                },
                skip() {
                    return !this.sessionKey
                },
                updateQuery(previousResult, { subscriptionData: { data: { display: { fieldData }}} })  {
                    this.errorMessage = null;
                    this.rows = processDisplayMessage(fieldData)
                },
                error(error) {
                    this.errorMessage = error
                }
            },
            variables() {
                return {sessionKey: this.sessionKey}
            },
            skip() {
                return !this.sessionKey
            },
            result({data: { display: { fieldData }}}) {
                this.errorMessage = null;
                this.rows = processDisplayMessage(fieldData)
            },
            error(error) {
                this.errorMessage = error
            }
        },
    },
    
    methods: {
        functionKey(aid) {
            const body = {
                sessionKey: this.sessionKey,
                fieldSubmission: {
                    aid: aid,
                }
            }

            this.$apollo.mutate({
                mutation: gqlConstants.submitFields,
                variables: { submission: body },
            });
        },

        enterKey() {

            const fieldSubmission = createFieldSubmission(this.cursor, this.rows)

            const body = {
                sessionKey: this.sessionKey,
                fieldSubmission: {
                    aid: Constants.AID.ENTER,
                    ...fieldSubmission
                }
            }

            this.$apollo.mutate({
                mutation: gqlConstants.submitFields,
                variables: { submission: body },

            });
        },

        inputChanged(row, col, value) {
            this.rows = inputValueChanged(this.rows, row, col, value)
        },

        focusChanged(row, col) {
            this.cursor = [row, col]
        },

        handleKeyDown(event) {
            if(event.key === 'Enter') {
                this.enterKey();
            }
        }
    },

    beforeUnmount() {
        window.removeEventListener('keydown', this.handleKeyDown);
    }
}
