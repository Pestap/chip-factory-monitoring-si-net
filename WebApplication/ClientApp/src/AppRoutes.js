import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
import { Home } from "./components/Home";
import Sensors from "./components/Sensors";
import Dashboard from "./components/Dashboard";
import SensorsChart from "./components/SensorsChart";


const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/sensors',
    element: <Sensors />
  },
  {
    path: '/dashboard',
    element: <Dashboard />
  },
  {
    path: '/chart',
    element: <SensorsChart />
  }
];

export default AppRoutes;
