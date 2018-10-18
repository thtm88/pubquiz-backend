import Vue from "vue";
import "./plugins/axios";
import App from "./App.vue";
import router from "./router/index";
//import signalr from "@aspnet/signalr";
import store from "./store/index.js";

Vue.config.productionTip = false;

new Vue({
  render: h => h(App),
  router,
  store
}).$mount("#app");
