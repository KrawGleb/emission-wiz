import { fileURLToPath, URL } from 'node:url';

import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [
        react({
            babel: {
                parserOpts: {
                    plugins: ['decorators']
                },
                plugins: [
                    [
                        '@babel/plugin-proposal-decorators',
                        {
                            version: '2023-05'
                        }
                    ]
                ]
            }
        })
    ],
    build: {
        sourcemap: true
    },
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    server: {
        port: process.env.PORT ? +process.env.PORT : 5173
    }
});
