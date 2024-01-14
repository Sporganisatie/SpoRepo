import { RouterProvider } from "react-router-dom";
import Layout from "./components/Layout";
import "./index.css";
import { StrictMode, useEffect, useState } from "react";
import { setupAxiosInterceptor } from "./AxiosInterceptor";
import { useNavigate } from "react-router-dom";
import router from "./Pages";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";

const queryClient = new QueryClient();

export default function App() {
  return (
    <StrictMode>
      <QueryClientProvider client={queryClient}>
        <RouterProvider router={router} />
      </QueryClientProvider>
    </StrictMode>
  );
}

export function Root() {
  const navigate = useNavigate();
  const [axiosInterceptorDone, setAxiosInterceptorDone] = useState(false);
  useEffect(() => {
    // onLoad
    setupAxiosInterceptor(navigate);
    setAxiosInterceptorDone(true);
  }, [navigate]);

  return axiosInterceptorDone ? <Layout /> : <></>;
}
