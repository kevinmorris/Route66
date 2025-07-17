export default {
    props: ['fieldData', 'cursor'],
    emits: ['inputChanged', 'focusChanged'],

    computed: {
        testId() {
            return `${this.fieldData.row}-${this.fieldData.col}`
        },

        positionStyle() {
            return {
                left: this.fieldData.col + 'ch',
                width: this.fieldData.length + 'ch',
            }
        }
    },
    mounted() {
        setTimeout(() => {
            if(this.fieldData.cursor > -1) {
                this.$refs.inputElement?.focus();
                this.$refs.inputElement?.setSelectionRange(0, 0);
            }
        }, 200)
    }
}
