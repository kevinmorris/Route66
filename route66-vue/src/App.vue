<script setup>
  import { watch } from 'vue';
  import { useConnectionStore } from '@/stores/connection.js';

  const connectionStore = useConnectionStore();

</script>

<template>
  <router-view />
</template>
