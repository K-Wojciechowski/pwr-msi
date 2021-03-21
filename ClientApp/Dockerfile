FROM node:current AS build-env
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . ./
RUN npm run build -- --prod

FROM nginx:alpine
COPY --from=build-env /app/dist /usr/share/nginx/html
COPY nginx/dev-nginx.conf /etc/nginx/conf.d/default.conf
