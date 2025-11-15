
# Use Node 20 (LTS) for React/Vite
FROM node:20

# Set working directory
WORKDIR /app

# Copy package files and install dependencies
COPY package*.json ./
RUN npm install

# Copy all project files
COPY . .

# Expose the Vite development server port
EXPOSE 5173

# Run the Vite dev server
CMD ["npm", "run", "dev", "--", "--host"]
# docker build -t react-app .