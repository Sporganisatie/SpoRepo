import { createTheme } from "react-data-table-component";

export const overrideDarkMode = () =>
  createTheme("dark", {
    background: {
      default: "#0f172a",
    },
    divider: {
      default: "#020617",
    },
    striped: {
      default: "#1e293b",
    },
    rows: {
      fontSize: "25px",
    },
  });
