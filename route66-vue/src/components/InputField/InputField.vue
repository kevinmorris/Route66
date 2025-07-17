<script lang="ts">
export { default } from './InputField.js'
</script>

<template>
  <input ref="inputElement"
         :style="positionStyle"
         :data-testid="testId"
         :value="fieldData.value"
         @input="(event) => $emit('inputChanged', fieldData.row, fieldData.col, event.target.value)"
         @focus="$emit('focusChanged', fieldData.row, fieldData.col)">
</template>

<style scoped>

</style>
