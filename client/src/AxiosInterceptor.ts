import axios, { AxiosError, AxiosResponse } from "axios"
import { AxiosRequestConfig } from "axios";
import { NavigateFunction } from "react-router-dom";

const onRequest = (config: AxiosRequestConfig): AxiosRequestConfig => {
    config.headers = config.headers ?? {};
    config.headers.Authorization = localStorage.getItem("authToken") ?? "";
    return config;
}

const onResponseError = (error: AxiosError, navigate: NavigateFunction): void => {
    switch (error.response?.status) {
        case 401: navigate("/login"); break;
        case 403: navigate("/home"); break;
    }
}

const setupAxiosInterceptor = (navigate: NavigateFunction) => {
    axios.interceptors.request.use(onRequest);
    axios.interceptors.response.use(res => res, error => onResponseError(error, navigate));
}

export { setupAxiosInterceptor }