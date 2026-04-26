import { defineConfig } from 'vite';

export default defineConfig({
  server: {
    proxy: {
      '/api/reservations': {
        target: 'http://localhost:5092',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/api\/reservations/, '/reservations')
      },
      '/api/guests': {
        target: 'http://localhost:5281',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/api\/guests/, '/guests')
      },
      '/api/rooms': {
        target: 'http://localhost:5187',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/api\/rooms/, '/rooms')
      },
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/api/, '')
      }
    }
  }
});
