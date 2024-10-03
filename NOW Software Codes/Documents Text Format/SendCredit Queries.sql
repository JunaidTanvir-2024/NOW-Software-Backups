
USE send_credit

update customer_order set order_status ='init' where customer_order_id=205

update customer_cart SET purchase_inventory_status ='', vendor_final_status ='' where customer_order_id=205


SELECT TOP 10 * FROM  dbo.customer_cart_item


SELECT TOP 10 * FROM dbo.users
WHERE email = 'Junaid.tanvir@now.net.pk'

SELECT TOP 10 * FROM dbo.customer_order
WHERE customer_id = 'b98dc0a9-b44a-496f-859d-0554e8b7e142'

SELECT TOP 10 * FROM dbo.customer_address
WHERE customer_address_id = 6

SELECT TOP 10 * FROM dbo.customer_cart_item
WHERE customer_cart_item_id = 10027

SELECT * FROM dbo.customer_address WHERE customer_address_id IN (1,6)