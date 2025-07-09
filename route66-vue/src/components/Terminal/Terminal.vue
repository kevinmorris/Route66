<script lang="js">
  export { default } from './Terminal.js'
</script>

<template>
  <div id="container" class="border-zero">
    <div>
      <button class="keypad-button">Reset</button>
      <button class="keypad-button" @click="functionKey(Constants.AID.PA1)">PA1</button>
      <button class="keypad-button" @click="functionKey(Constants.AID.PA2)">PA2</button>
      <button class="keypad-button" @click="functionKey(Constants.AID.PA3)">PA3</button>
      <button class="keypad-button" @click="functionKey(Constants.AID.SYSREQ)">Sys Req</button>
      <button class="keypad-button" @click="functionKey(Constants.AID.CLEAR)">Clear</button>
    </div>
    <div>
      <button class="keypad-button" @click="functionKey(Constants.AID.PF1)">PF1</button>
      <button class="keypad-button" @click="functionKey(Constants.AID.PF2)">PF2</button>
      <button class="keypad-button" @click="functionKey(Constants.AID.PF3)">PF3</button>
      <button class="keypad-button" @click="functionKey(Constants.AID.PF4)">PF4</button>
      <button class="keypad-button" @click="functionKey(Constants.AID.PF5)">PF5</button>
      <button class="keypad-button" @click="functionKey(Constants.AID.PF6)">PF6</button>
      <button class="keypad-button" @click="functionKey(Constants.AID.PF7)">PF7</button>
      <button class="keypad-button" @click="functionKey(Constants.AID.PF8)">PF8</button>
      <button class="keypad-button" @click="functionKey(Constants.AID.PF9)">PF9</button>
      <button class="keypad-button" @click="functionKey(Constants.AID.PF10)">PF10</button>
      <button class="keypad-button" @click="functionKey(Constants.AID.PF11)">PF11</button>
      <button class="keypad-button" @click="functionKey(Constants.AID.PF12)">PF12</button>
    </div>
    <div>
      <Row v-for="(row, i) in rows"
           :key="i"
           :i="i"
           :fieldData="row"
           @inputChanged="inputChanged"
           @focusChanged="focusChanged">
      </Row>
    </div>
  </div>
</template>

<style scoped>

</style>
