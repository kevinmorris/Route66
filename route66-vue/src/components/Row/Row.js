import InputField from "@/components/InputField/InputField.vue";
import LabelField from "@/components/LabelField/LabelField.vue";

export default {
    props: ['i', 'fieldData'],
    emits: ['inputChanged', 'focusChanged'],

    components: {
        InputField,
        LabelField
    }
}
