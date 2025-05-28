FROM node:20-alpine
WORKDIR /app

COPY package*.json ./
RUN npm install -g @angular/cli
RUN npm ci
COPY . .

ARG API_URL
ENV API_URL=$API_URL
ENV NG_CLI_ANALYTICS=false

RUN npm run generate-env
RUN npm run build

EXPOSE 4200
CMD ["ng", "serve", "--host", "0.0.0.0"]
