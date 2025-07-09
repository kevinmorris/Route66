<script lang="ts">
export { default } from './InputField.js'
</script>

<template>
  <input :style="positionStyle"
         :data-testid="testId"
         @input="(event) => $emit('inputChanged', fieldData.row, fieldData.col, event.target.value)"
         @focus="$emit('focusChanged', fieldData.row, fieldData.col)">
</template>

<style scoped>

</style>
