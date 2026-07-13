import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import fs from 'fs'
import path from 'path'

// Plugin to serve Unity WebGL Brotli-compressed build files with correct HTTP headers
function unityWebglBrotliPlugin() {
  return {
    name: 'unity-webgl-brotli',
    configureServer(server: any) {
      server.middlewares.use((req: any, res: any, next: any) => {
        const url: string = req.url || ''

        // Map Unity WebGL requests to their .br equivalents
        const brotliMap: Record<string, { file: string; contentType: string }> = {
          '/Build/Build.framework.js': {
            file: 'Build.framework.js.br',
            contentType: 'application/javascript',
          },
          '/Build/Build.data': {
            file: 'Build.data.br',
            contentType: 'application/octet-stream',
          },
          '/Build/Build.wasm': {
            file: 'Build.wasm.br',
            contentType: 'application/wasm',
          },
        }

        const match = brotliMap[url]
        if (match) {
          const filePath = path.join(__dirname, 'public', 'Build', match.file)
          if (fs.existsSync(filePath)) {
            res.setHeader('Content-Encoding', 'br')
            res.setHeader('Content-Type', match.contentType)
            res.setHeader('Cache-Control', 'no-cache')
            fs.createReadStream(filePath).pipe(res)
            return
          }
        }

        next()
      })
    },
  }
}

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), unityWebglBrotliPlugin()],
})

