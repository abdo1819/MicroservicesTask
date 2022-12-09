# floward homework

this repository contains the homework for the floward task
# content
- [x] [Microsserice Desigen](##microsserice-desigen)
- [ ] [desigen pattern i am goining to use](##used patterns)
- [ ] [.net core demo application](##application demo)
- [ ] [kafka intergration](##Kafaka integration)
- [ ] [unit testing](##unit testing)


## microsserice-desigen
<!-- missing components -->
<!-- logging  -->
<!-- service monitoring -->
<!-- zoo keeper -->

## services
- product service <!-- - cqrs  -->
- product search
- order service <!-- - saga  -->
- cart service 

## product service
manage products data with oltp database (sql server) 
that maintatin constrains and relations between tables
peroform update and addition on products data

### interface
- `POST /product/id` add new product
- `DELETE /product/id` delete product
- `PUT /product/id` update product

### topics actions
#### publish
- `product.added` publish when new product added
- `product.deleted` publish when product deleted
- `product.updated` publish when product updated

#### subscribe
- `cart.checkout` subscribe when cart checkout to decrease product quantity
- `order.cancel` subscribe when order canceled to increase product quantity


## product search
perform search on products data with olap database (elasticsearch)
that can be used for search and analytics

### interface
- `GET /product/search` search for products

### topics actions
#### publish
- none

#### subscribe
- `product.added` subscribe when new product added
- `product.deleted` subscribe when product deleted
- `product.updated` subscribe when product updated

## order service
manage orders data with oltp database (sql server)
use saga pattern to handle order process

### interface
- `POST /order/id/cancel` cancel order
- `GET /order/id` get order

### topics actions
#### publish
- `order.cancel` publish when order canceled


#### subscribe
- `cart.checkout` subscribe when cart checkout to create order
- `payment.succeeded` subscribe when payment succeeded to confirm order
- `payment.failed` subscribe when payment failed to cancel order

## cart service
manage cart data with temporal database (redis)

### interface
- `POST /cart/id/checkout` checkout cart
- `GET /cart/id` get cart
- `POST /cart/id/product/id` add product to cart
- `DELETE /cart/id/product/id` remove product from cart

### topics actions
#### publish
- `cart.checkout` publish when cart checkout

#### subscribe
- `product.price.updated` subscribe when product price updated to update cart price

## payment service
manage payment data with persistent database (sql server)

### interface
- `POST /payment/id/succeeded` confirm payment
- `POST /payment/id/failed` cancel payment

### topics actions
#### publish
- `payment.succeeded` publish when payment succeeded
- `payment.failed` publish when payment failed

#### subscribe
- `cart.checkout` subscribe when cart checkout to create payment invoice

## topics
- `product.added` publish when new product added
- `product.deleted` publish when product deleted
- `product.updated` publish when product updated
- `product.price.updated` publish when product price updated
- `cart.checkout` publish when cart checkout
- `payment.succeeded` publish when payment succeeded
- `payment.failed` publish when payment failed
- `order.cancel` publish when order canceled

### event chronology sample 
- user checkout 
```
cart.checkout 
|
|--> payment create invoice
|
|--> order create invalid order
|
|--> warehouse reserve product
|
|--> product update product quantity
```

```
payment.succeeded
|
|--> order confirm order
|
|--> warehouse commit shipping , decrease product quantity
```

```
payment.failed
|
|--> order cancel order
|
|--> warehouse remoce reservation
```

```
order.cancel
|
|--> warehouse increase product quantity
|
|--> payment cancel invoice / issue refund
|
|--> product update product quantity
```




## database
- sql server
- elasticsearch
- redis

## api gateway
- nginx

## service discovery
- consul
