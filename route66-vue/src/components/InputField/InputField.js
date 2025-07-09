export default {
    props: ['fieldData'],
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
}
