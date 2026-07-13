# Use a lightweight official Node.js image
FROM node:20-alpine

# Set the working directory inside the container
WORKDIR /app

# Copy package files first to leverage Docker layer caching
COPY package.json package-lock.json ./

# Install only production dependencies (excluding devDependencies like jest, nodemon)
RUN npm ci --omit=dev

# Copy the rest of the application source code
COPY src ./src

# Expose the port the backend runs on
EXPOSE 5000

# Set production environment defaults
ENV PORT=5000
ENV NODE_ENV=production

# Start the application
CMD ["npm", "start"]
