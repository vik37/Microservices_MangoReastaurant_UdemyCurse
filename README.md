
# Mango Restaurant

It's about an online food delivery application. Developed in microservices architecture using Dot Net stack technologies: Dot Net MVC (Web), Dot Net API (Services), Ocelot (Gateway), Microsoft SQL (Database), in all services also ORM Entity Framework and secure with Identity Server 5. The application was built during the study period of Microservice Architecture with Dot Net 6.


## Product API

#### Get all products

```http
  GET /api/product
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `api_key` | `null` | **Required**. No |

#### Get product by id

```http
  GET /api/product/${id}
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `id`      | `integer` | **Required**. Id of product to fetch |

#### Add product

```http
  POST /api/product
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `product`      | `ProductDto` | **Required**. Product model |

#### Update product

```http
  PUT /api/product
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `product`      | `ProductDto` | **Required**. Product model |

#### Remove product by id

```http
  DELETE /api/product/${id}
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `id`      | `integer` | **Required**. Id of product to delete |

## Shopping Cart API

#### Get cart

```http
  GET /api/cart/getcart/${userId}
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `userId`      | `string` | **Required**. User id to fetch the cart|

#### Add cart

```http
  POST /api/cart/addcart
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `cartDto`      | `CartDto` | **Required**. Cart model |

#### Update cart

```http
  PUT /api/cart/updatecart
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `cartDto`      | `CartDto` | **Required**. Cart model |

#### Delete cart

```http
  DELETE /api/cart/removecart/${cartId}
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `cartId`      | `integer` | **Required**. Id of cart to delete |

#### Apply for coupon cart

```http
  POST /api/cart/applycoupon
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `cartDto`      | `CartDto` | **Required**. Cart model |

#### Remove coupon cart

```http
  POST /api/cart/removecoupon/${userId}
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `userId`      | `string` | **Required**. userId for delete coupon |

#### Clear the cart by user

```http
  POST /api/cart/clearcart/${userId}
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `userId`      | `string` | **Required**. userId for clear the cart for that user |

#### Checkout

```http
  POST /api/cart/checkout
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `checkoutHeader`      | `CheckoutHeaderDto` | **Required**. checkout model |

## Coupon API

#### Get the coupon discount products code

```http
  GET /api/coupon/${code}
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `code`      | `string` | **Required**. code for how much discount is it 30% or 20%  |





## Appendix

Order API, Payment API, Email - works by events using **Message Broker** 
- RabbitMQ, 
- Azzure: Service Buss Message (1. **Queue**, 2. **Topics**);

- Images are in the Azure Cloud Storage. 


## Badges

Add badges from somewhere like: [shields.io](https://shields.io/)

[![GitHub language count](https://img.shields.io/github/languages/count/vik37/Microservices_MangoReastaurant_UdemyCurse?style=plastic)](https://img.shields.io/github/languages/count/vik37/Microservices_MangoReastaurant_UdemyCurse?style=plastic)

[![GitHub top language](https://img.shields.io/github/languages/top/vik37/Microservices_MangoReastaurant_UdemyCurse)](https://img.shields.io/github/languages/top/vik37/Microservices_MangoReastaurant_UdemyCurse)

[![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/vik37/Microservices_MangoReastaurant_UdemyCurse)](https://img.shields.io/github/languages/code-size/vik37/Microservices_MangoReastaurant_UdemyCurse)

![GitHub repo size](https://img.shields.io/github/repo-size/vik37/Microservices_MangoReastaurant_UdemyCurse?color=%23556D88&logoColor=pink&style=plastic)

![Nuget](https://img.shields.io/nuget/v/Swashbuckle.AspNetCore.Swagger?logoColor=red&style=plastic)

![GitHub commit activity](https://img.shields.io/github/commit-activity/y/vik37/Microservices_MangoReastaurant_UdemyCurse?color=FF5733)

![GitHub last commit](https://img.shields.io/github/last-commit/vik37/Microservices_MangoReastaurant_UdemyCurse)

[![YouTube Channel Views](https://img.shields.io/youtube/channel/views/UCI9Izr8SBlnBcORRSgXAspg?style=social)](https://www.youtube.com/watch?v=sKHc-ipv_b4&t=1s)


## 🔗 Links

[![linkedin](https://img.shields.io/badge/linkedin-0A66C2?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/viktor-zafirovski-8165725a/)

[![twitter](https://img.shields.io/badge/twitter-1DA1F2?style=for-the-badge&logo=twitter&logoColor=white)](https://twitter.com/ViktorZafirovs1)


## Demo

https://www.youtube.com/watch?v=sKHc-ipv_b4


## Work flow

![App Screenshot](https://raw.githubusercontent.com/vik37/Microservices_MangoReastaurant_UdemyCurse/master/Diagram/microservices_flow.jpg)


## Run Locally

Clone the project

```bash
  git clone https://github.com/vik37/Microservices_MangoReastaurant_UdemyCurse.git
```

Go to the project directory

```bash
  cd Microservices_MangoReastaurant_UdemyCurse 
```

Build the app

```bash
  dotnet build
```

Start the app

```bash
  dotnet run
```


![Logo](https://raw.githubusercontent.com/vik37/Microservices_MangoReastaurant_UdemyCurse/master/Mango.Web/wwwroot/images/mango.png)


## Tech Stack

**Client:** Dot Net 6.0 MVC

**Server:** Dot Net Web API

**Message Broker:** RabbitMQ, Azure Service Buss

**Database:** Microsoft SQL

**ORM:** Entity Framework

**App Secure (Authentication, Authorization):** Identity Server (**OAuth, OIDC**)
- Identity User
- JWT Token Bearer


## Authors

- [Viktor Zafirovski](https://github.com/vik37)

