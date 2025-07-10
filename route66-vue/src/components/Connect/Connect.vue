<script lang="js">
  export { default } from './Connect.js'
</script>

<template>
  <h1>Route 66 Connect</h1>
  <form>
    <div class="field">
      <span>Address:</span>
      <input type="text" name="address" v-model="address"/>
    </div>
    <div class="field">
      <span>Port:</span>
      <input type="text" name="port" v-model="port"/>
    </div>
    <button type="button" @click="login">Submit</button>
  </form>
</template>

<style scoped>

</style>
