# Stage 1: Build the React application
FROM node:20-alpine AS builder

WORKDIR /app

# Copy package files first for caching
COPY package.json package-lock.json ./

# Install all dependencies (needed for compilation and build)
RUN npm ci

# Copy the rest of the application source code
COPY . .

# Build the project (generates the dist directory)
RUN npm run build

# Stage 2: Serve the application with Nginx
FROM nginx:alpine

# Copy custom Nginx configuration to support Unity WebGL Brotli-compressed files
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy build output from the builder stage
COPY --from=builder /app/dist /usr/share/nginx/html

# Expose port 80
EXPOSE 80

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]
