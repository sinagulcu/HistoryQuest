/// <reference types="vite/client" />

import "axios";

declare module "axios" {
  interface AxiosRequestConfig {
    skipAuth?: boolean;
    autoLogoutOn401?: boolean;
  }
}