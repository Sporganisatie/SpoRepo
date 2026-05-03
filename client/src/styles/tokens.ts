/**
 * Design tokens mirroring src/styles/tokens.css.
 *
 * The CSS variables in tokens.css are the source of truth for plain CSS
 * and styled-components consumers. This TypeScript object is for places
 * that need a runtime color value (recharts series colors, the
 * react-data-table-component theme bridge, etc.). Keep both in sync.
 */
export const tokens = {
  bg: {
    page: "#020617",
    surface: "#0f172a",
    elevated: "#1e293b",
    control: "#334155",
    controlHover: "#475569",
    disabled: "#64748b",
  },
  fg: {
    default: "#ffffff",
    muted: "#d4d4d8",
    onLight: "#000000",
  },
  chart: {
    bg: "#222222",
    grid: "#333333",
    axis: "#cccccc",
  },
  accent: {
    blue: "#3b82f6",
    red: "#ef4444",
    redDark: "#b91c1c",
    green: "#15803d",
    greenMid: "#10b981",
    greenDark: "#047857",
  },
  podium: {
    gold: "#fde047",
    silver: "#e2e8f0",
    bronze: "#fbbf24",
  },
} as const;

export type Tokens = typeof tokens;
