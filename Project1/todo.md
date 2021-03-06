# Session Header
- [X] Change data based on login status
- [X] "Not logged in" state
- [X] "Logged in" state
- [X] Use data from db
- [X] styles

# Storefront page
- [X] Display products from selected store or default store
- [X] Redirect to account management page on product click if not logged in
- [X] Show which store's items are being displayed
- [X] Click items to go to detail page
- [X] Use data from db
- [X] styles

# Account Management Page
- [X] Use data from db
- [X] Order history link
- [X] Store selector
- [X] Personal info form
- [X] Save personal info
- [X] styles

# Order History page
- [X] Use data from db
- [X] List orders
- [X] Display details of order
- [X] styles

# Item detail page
- [X] Display product
- [X] Quantity picker and 'add to cart' button
- [X] Display message if item not found
- [X] Update order info on backend
- [X] Use data from db
- [X] Disallow ordering more than the max
- [X] styles

# Added to cart page
- [X] Display product, quantity, and message
- [X] Use data from db
- [X] 'Checkout' button
- [X] 'Continue Shopping' button
- [X] styles

# Item detail page
- [X] link to try again on error

# Checkout page
- [X] Display products in cart, quantity, and totals per product
- [X] Display total for order
- [X] 'Purchase' button
- [X] Use data from db
- [X] Display different information when cart is empty
- [X] Remove purchase button when cart empty
- [X] styles

## Purchase OK page
## Purchase error page
- [X] Link to cart
- [X] styles

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
- [X] styles

# Error page
- [X] stlyes

# Admin pages
- [X] view location orders
- [X] search for customers
- [X] styles

# General
- [X] Add redirects for POST-only URLs to appropriate location (cart main, storefront, etc)
- [X] fix logger generics in all controllers
- [X] apply async where appropriate
- [X] autofocus
- [X] ensure appropriate routes are authorize-gated
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
- [X] style flash messages
- [X] style access denied page
- [ ] remove .AsEnumerable calls in repos
- [ ] use implicit interface implementations
- [X] remove console writelines
- [X] add ValidateAntiForgeryToken on POST requests

# Missing Requirements
- [X] logging
- [X] docs
