# LibreCuisine
<!-- ALL-CONTRIBUTORS-BADGE:START - Do not remove or modify this section -->
[![All Contributors](https://img.shields.io/badge/all_contributors-1-orange.svg?style=flat-square)](#contributors-)
<!-- ALL-CONTRIBUTORS-BADGE:END -->

[![Licence: APGLv3](https://img.shields.io/github/license/LibreCuisine/LibreCuisine "Licence: APGLv3")][AGPL3]
[![Contributors](https://badgen.net/github/contributors/LibreCuisine/LibreCuisine)][Contributors]
[![Release](https://badgen.net/github/release/LibreCuisine/LibreCuisine/stable)][Release]

LibreCuisine is a open source cooking recipe web application build on a mircoservice infrastructure

[AGPL3]: https://www.gnu.org/licenses/agpl-3.0
[Contributors]: https://github.com/LibreCuisine/LibreCuisine/graphs/contributors
[Release]: https://github.com/LibreCuisine/LibreCuisine/releases

## Build Status (Github Actions)
| Image      | Status                                                                                                                                     |
|------------|--------------------------------------------------------------------------------------------------------------------------------------------|
| Recipe API | [![recipe-api](https://github.com/LibreCuisine/LibreCuisine/actions/workflows/recipe-api.yml/badge.svg?branch=develop)][recipe-api-action] |

[recipe-api-action]: https://github.com/LibreCuisine/LibreCuisine/actions/workflows/recipe-api.yml

## Getting Started

For the authentication we need to create a public and private key with `openssl`

```sh
mkdir -p ~/.librecuisine
openssl genrsa -out ~/.librecuisine/privatekey.pem
openssl rsa -in ~/.librecuisine/privatekey.pem -out ~/.librecuisine/publickey.pem -pubout -outform PEM
```

Replace paths all `appsettings.Development.json` with the path absolute paths to the public and private keys created in the step early
**!Important, don't use `~` for the home path as dotnet doesn't recognize that**

Make sure to have docker installed and configured

```sh
cd src
docker-compose build
docker-compose up
```

### Architecture

```mermaid
flowchart LR
  subgraph Clients
  webapp[WebApp\nReact]
  website[Website]
  end

  webmvc[WebMvc]
  gw-api{Api Gateway}

  webapp-->gw-api
  website-->webmvc
  webmvc-->gw-api

  gw-api-->id-api
  gw-api-->r-api
  gw-api-->p-api

  subgraph .
  rmq[RabbitMQ]

  subgraph Identity
  id-api[Identity.Api]
  id-api<-->id-data(Postgre)
  end

  subgraph Profile
  p-api[Profile.Api]
  p-api<-->data(Database)
  end

  subgraph Recipe
  r-api[Recipe.Api]
  r-api<-->r-data(MongoDB)
  end
  r-api--->|async|rmq
  p-api<---|async|rmq

  r-api-->|gRPC|p-api
  end
```

## Contributors

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tbody>
    <tr>
      <td align="center"><a href="https://github.com/jasperspahl"><img src="https://avatars.githubusercontent.com/u/39385451?v=4?s=100" width="100px;" alt="Jasper Spahl"/><br /><sub><b>Jasper Spahl</b></sub></a><br /><a href="https://github.com/LibreCuisine/LibreCuisine/commits?author=jasperspahl" title="Code">💻</a> <a href="#projectManagement-jasperspahl" title="Project Management">📆</a></td>
    </tr>
  </tbody>
</table>

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
