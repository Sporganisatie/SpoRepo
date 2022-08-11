import axios from "axios"
import { AxiosRequestConfig } from "axios";

const onRequest = (config: AxiosRequestConfig): AxiosRequestConfig => {
    config.headers = config.headers ?? {};
    config.headers.Authorization = localStorage.getItem("authToken") ?? "";
    return config;
}

const setupAxiosInterceptor = () => {
    axios.interceptors.request.use(onRequest);
}

export { setupAxiosInterceptor }