export default {
    props: ['fieldData'],

    computed: {
        testId() {
            return `${this.fieldData.row}-${this.fieldData.col}`
        },

        positionStyle() {
            return {
                left: this.fieldData.col + 'ch',
                position: 'absolute'
            }
        }
    }
}
