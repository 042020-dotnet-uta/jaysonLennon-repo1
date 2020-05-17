# Session Header
- [X] Change data based on login status
- [X] "Not logged in" state
- [X] "Logged in" state
- [X] Use data from db
- [ ] styles

# Storefront page
- [X] Display products from selected store or default store
- [X] Redirect to account management page on product click if not logged in
- [X] Show which store's items are being displayed
- [X] Click items to go to detail page
- [X] Use data from db
- [ ] styles

# Account Management Page
- [X] Use data from db
- [X] Order history link
- [X] Store selector
- [X] Personal info form
- [X] Save personal info
- [ ] styles

# Order History page
- [X] Use data from db
- [X] List orders
- [X] Display details of order
- [ ] styles


# Item detail page
- [X] Display product
- [X] Quantity picker and 'add to cart' button
- [X] Display message if item not found
- [X] Update order info on backend
- [X] Use data from db
- [X] Disallow ordering more than the max
- [ ] styles

# Added to cart page
- [X] Display product, quantity, and message
- [X] Use data from db
- [X] 'Checkout' button
- [X] 'Continue Shopping' button
- [ ] styles

# Item detail page
- [ ] link to try again on error

# Checkout page
- [X] Display products in cart, quantity, and totals per product
- [X] Display total for order
- [X] 'Purchase' button
- [X] Use data from db
- [X] Display different information when cart is empty
- [X] Remove purchase button when cart empty
- [ ] styles

## Purchase OK page
## Purchase error page
- [ ] Link to cart
- [ ] styles

# View cart page
- [X] List products
- [X] quantity boxes
- [X] remove buttons
- [X] "update quantities" button
- [X] "update quantities" redirects to cart view when session expires
- [X] Disallow ordering more than the max
- [X] show maximum possible to order for each item
- [X] show different data when the cart is empty
- [X] display error on broken model
- [X] "Checkout" link
- [X] Use data from db
- [ ] styles

# Error page
- [ ] stlyes

# Admin pages
- [X] view location orders
- [X] search for customers
- [ ] stlyes

# General
- [X] Add redirects for POST-only URLs to appropriate location (cart main, storefront, etc)
- [X] fix logger generics in all controllers
- [ ] apply async where appropriate
- [ ] autofocus
- [ ] ensure appropriate routes are authorize-gated
- [X] remove unused imports
- [X] separate models into view and submission
- [X] grep for unimplemented interfaces
- [X] exception handling (drop the db to test)
- [X] require customer ids in queries
- [X] model binding check
- [X] use tempdata for informational message passing
- [X] post requests should redirect to get
- [X] add flash message display to all pages
- [ ] add 404 page
- [ ] style flash messages
- [ ] style access denied page
- [ ] remove .AsEnumerable calls in repos
- [ ] use implicit interface implementations
- [ ] remove console writelines
- [ ] add ValidateAntiForgeryToken on POST requests

