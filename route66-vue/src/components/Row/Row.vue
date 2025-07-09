<script lang="js">
  export { default } from './Row.js'
</script>

<template>
  <div class="row">
    <template v-for="field in fieldData" v-bind:key="`${i}-${field.col}`">
      <LabelField v-if="field.isProtected" :fieldData="field"/>
      <InputField v-else
                  :fieldData="field"
                  @inputChanged="(row, col, value) => $emit('inputChanged', row, col, value)"
                  @focusChanged="(row, col) => $emit('focusChanged', row, col)"/>
    </template>
  </div>
</template>

<style scoped>

</style>
