import { defineConfig, loadEnv } from "vite";
import react from "@vitejs/plugin-react";
import fs from "node:fs";
import path from "node:path";

export default defineConfig(({ mode }) => {
  const env = { ...process.env, ...loadEnv(mode, process.cwd(), "") };

  const apiTarget = env.ASPNETCORE_HTTPS_PORT
    ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
    : env.ASPNETCORE_URLS
      ? env.ASPNETCORE_URLS.split(";")[0]
      : "http://localhost:40332";

  const https =
    env.SSL_CRT_FILE &&
      env.SSL_KEY_FILE &&
      fs.existsSync(env.SSL_CRT_FILE) &&
      fs.existsSync(env.SSL_KEY_FILE)
      ? {
        cert: fs.readFileSync(env.SSL_CRT_FILE),
        key: fs.readFileSync(env.SSL_KEY_FILE),
      }
      : undefined;

  return {
    plugins: [react()],
    resolve: {
      alias: { "@": path.resolve(__dirname, "src") },
    },
    server: {
      port: env.PORT ? Number(env.PORT) : 44404,
      open: env.BROWSER === "none" ? false : undefined,
      https,
      proxy: {
        "/api": {
          target: apiTarget,
          secure: false,
          changeOrigin: true,
          headers: { Connection: "Keep-Alive" },
        },
        "/profiles": {
          target: apiTarget,
          secure: false,
          changeOrigin: true,
          headers: { Connection: "Keep-Alive" },
        },
      },
    },
    build: {
      outDir: "build",
      sourcemap: true,
    },
  };
});
