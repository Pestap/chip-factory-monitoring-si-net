import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
import { Home } from "./components/Home";
import Sensors from "./components/Sensors";
import Dashboard from "./components/Dashboard";

import { createWebSocketContext } from "react-signalr/websocket";

import { createSignalRContext } from "react-signalr/signalr";

const SignalRContext = createSignalRContext();

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/counter',
    element: <Counter />
  },
  {
    path: '/fetch-data',
    element: <FetchData />
  },
  {
    path: '/sensors',
    element: <Sensors />
  },
  {
    path: '/dashboard',
    element: <Dashboard />
  }
];

export default AppRoutes;
