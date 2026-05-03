import { createTheme } from "react-data-table-component";
import { tokens } from "../../../styles/tokens";

export const overrideDarkMode = () =>
  createTheme("dark", {
    background: {
      default: tokens.bg.surface,
    },
    divider: {
      default: tokens.bg.page,
    },
    striped: {
      default: tokens.bg.elevated,
    },
    rows: {
      fontSize: "25px",
    },
  });
