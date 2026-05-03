import type { AxiosError, AxiosRequestConfig } from "axios";
import type { NavigateFunction } from "react-router-dom";
import { apiClient } from "./client";

const onRequest = (config: AxiosRequestConfig): AxiosRequestConfig => {
  config.headers = config.headers ?? {};
  config.headers.Authorization = localStorage.getItem("authToken") ?? "";
  return config;
};

const onResponseError = (error: AxiosError, navigate: NavigateFunction): void => {
  switch (error.response?.status) {
    case 401:
      navigate("/login");
      break;
    case 403:
    case 423:
      navigate("/");
      break;
  }
};

export const setupAxiosInterceptor = (navigate: NavigateFunction) => {
  apiClient.interceptors.request.use(onRequest);
  apiClient.interceptors.response.use(
    (res) => res,
    (error) => onResponseError(error, navigate)
  );
};
