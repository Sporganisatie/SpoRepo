import { Route, Routes } from "react-router-dom";
import Layout from "./components/Layout";
import Pages from "./Pages";
import "./index.css";
import { useEffect, useState } from "react";
import { setupAxiosInterceptor } from "./AxiosInterceptor"
import { useNavigate } from "react-router-dom";

const App = () => {
  let navigate = useNavigate();
  const [axiosInterceptorDone, setAxiosInterceptorDone] = useState(false)
  useEffect(() => { // onLoad
    setupAxiosInterceptor(navigate);
    setAxiosInterceptorDone(true)
  }, [navigate]);

  return (
    <Routes>
      {/* Routes kunnen we wss makkelijk opsplitsen in public Reactroute (betere naam) en admin routes */}
      <Route path="/" element={<Layout />}>
        {axiosInterceptorDone && Pages}
      </Route>
    </Routes>
  );
};

export default App;
