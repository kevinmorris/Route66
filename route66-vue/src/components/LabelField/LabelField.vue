<script lang="js">
export { default } from './LabelField.js'
</script>

<template>
  <span :style="positionStyle" :data-testid="testId">
      {{ fieldData.value }}
  </span>
</template>

<style scoped>

</style>
