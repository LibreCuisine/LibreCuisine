FROM golang:1.19.4 AS builder
WORKDIR /src
COPY go.mod go.sum ./
RUN go mod download -x

COPY . ./
RUN CGO_ENABLED=0 GOOS=linux GOARCH=amd64 go build -a -installsuffix cgo -ldflags="-w -s" -v -o /build/app .

FROM scratch
COPY --from=builder /src/templates /templates
COPY --from=builder /src/static /static
COPY --from=builder /build /app
ENTRYPOINT [ "/app" ]
