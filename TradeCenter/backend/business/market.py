from .user import UserFacade
from .authentication.authentication import Authentication
from .roles import RolesFacade
from .DTOs import NotificationDTO, PurchaseDTO, PurchaseProductDTO
from .store import StoreFacade
from .purchase import PurchaseFacade
from .ThirdPartyHandlers import PaymentHandler, SupplyHandler
from .notifier import Notifier
from typing import List, Dict, Tuple, Optional
from datetime import datetime
import threading
import logging

logger = logging.getLogger('myapp')


def add_payment_method(method_name: str, payment_config: Dict):
    PaymentHandler().add_payment_method(method_name, payment_config)


class MarketFacade:
    # singleton
    __instance = None
    __lock = threading.Lock()

    def __new__(cls):
        if MarketFacade.__instance is None:
            MarketFacade.__instance = object.__new__(cls)
        return MarketFacade.__instance

    def __init__(self):
        if not hasattr(self, '_initialized'):
            self._initialized = True

            # initialize all the facades
            self.user_facade = UserFacade()
            self.store_facade = StoreFacade()
            self.roles_facade = RolesFacade()
            self.purchase_facade = PurchaseFacade()
            self.addresses = []
            self.auth_facade = Authentication()
            self.notifier = Notifier()

            # create the admin?
            self.__create_admin()

    def __create_admin(self, currency: str = "USD") -> None:
        man_id = self.user_facade.create_user(currency)
        hashed_password = self.auth_facade.hash_password("admin")
        self.user_facade.register_user(man_id, "admin@admin.com", "admin", hashed_password,
                                       2000, 1, 1, "123456789")
        self.roles_facade.add_admin(man_id)

    def clean_data(self):
        """
        For testing purposes only
        """
        self.user_facade.clean_data()
        self.store_facade.clean_data()
        self.roles_facade.clean_data()
        PaymentHandler().reset()
        SupplyHandler().reset()

        # create the admin?
        self.__create_admin()

    def show_notifications(self, user_id: int) -> List[NotificationDTO]:
        return self.user_facade.get_notifications(user_id)

    def add_product_to_basket(self, user_id: int, store_id: int, product_id: int):
        with MarketFacade.__lock:
            if self.store_facade.check_product_availability(store_id, product_id):
                self.user_facade.add_product_to_basket(user_id, store_id, product_id)

    def checkout(self, user_id: int, payment_details: Dict, supply_method: str, address: Dict):
        products_removed = False
        purchase_accepted = False
        basket_cleared = False
        cart: Dict[int, Dict[int, int]] = {}  # store_id -> product_id -> amount
        pur_id = -1
        try:
            # needs a whole revamp to work with the discounts and purchase policies and location restrictions
            # cart = self.user_facade.get_shopping_cart(user_id)
            # TODO: call actual user facade method

            # lock the __lock
            # check if the products are still available
            for store_id, products in cart.items():
                for product_id in products:
                    amount = products[product_id]
                    if not self.store_facade.check_product_availability(store_id, product_id, amount):
                        raise ValueError(f"Product {product_id} is not available in the required amount")

            """
            self.__product_id: int = product_id
            self.__name: str = name
            self.__description: str = description
            self.__price: float = price
            self.__amount: int = amount"""

            # calculate the total price
            purchase_shopping_cart: Dict[int, Tuple[List[PurchaseProductDTO], float, float]] = {}
            total_price = 0
            for store_id, products in cart.items():
                basket_price = 0
                purchase_products: List[PurchaseProductDTO] = []
                for product_id in products:
                    amount = products[product_id]
                    name = self.store_facade.get_product_by_id(product_id).name
                    description = self.store_facade.get_product_by_id(product_id).description
                    price = self.store_facade.get_product_by_id(product_id).price
                    basket_price += price * amount
                    purchase_products.append(PurchaseProductDTO(product_id, name, description, price, amount))
                purchase_shopping_cart[store_id] = (purchase_products, basket_price, basket_price)
                total_price += basket_price

            # purchase facade immediate
            total_price_after_discounts = self.store_facade.get_total_price_after_discount(cart)
            pur_id = self.purchase_facade.create_immediate_purchase(user_id, total_price, total_price_after_discounts,
                                                                    purchase_shopping_cart)

            # remove the products from the store
            for store_id, products in cart.items():
                for product_id in products:
                    amount = products[product_id]
                    self.store_facade.remove_product_amount(store_id, product_id, amount)

            products_removed = True

            # calculate the policies of the purchase using storeFacade + user location constraints
            for store_id in cart:
                products: Dict[int, int] = cart[store_id]
                # TODO: add check of policy
                """if not self.store_facade.check_policies_of_store(basket[0], basket[1]):
                    # self.purchase_facade.invalidate_purchase_of_user_immediate(purchase.purchase_id, user_id)
                    raise ValueError("Purchase does not meet the store's policies")"""

            # TODO: attempt to find a delivery method for user
            package_details = {'stores': cart.keys(), "supply method": supply_method}
            delivery_date = SupplyHandler().get_delivery_time(package_details, address)

            # accept the purchase
            self.purchase_facade.accept_purchase(pur_id, delivery_date)
            purchase_accepted = True

            # clear the cart
            self.user_facade.clear_basket(user_id)
            basket_cleared = True

            # TODO: (next version) fix discounts
            if "payment method" not in payment_details:
                # self.purchase_facade.invalidate_purchase_of_user_immediate(purchase.purchase_id, user_id)
                raise ValueError("Payment method not specified")

            if not PaymentHandler().process_payment(total_price_after_discounts, payment_details):
                # invalidate Purchase
                # self.purchase_facade.invalidate_purchase_of_user_immediate(purchase.purchase_id, user_id)
                raise ValueError("Payment failed")

            package_details = {'shopping cart': cart, 'address': address, 'arrival time': delivery_date,
                               'purchase id': pur_id, "supply method": supply_method}
            if "supply method" not in package_details:
                raise ValueError("Supply method not specified")
            if package_details.get("supply method") not in SupplyHandler().supply_config:
                raise ValueError("Invalid supply method")
            on_arrival = lambda purchase_id: self.purchase_facade.complete_purchase(purchase_id)
            SupplyHandler().process_supply(package_details, user_id, on_arrival)
            for store_id in cart.keys():
                Notifier().notify_new_purchase(store_id, user_id)
        except Exception as e:
            if products_removed:
                for store_id, products in cart.items():
                    for product_id in products:
                        amount = products[product_id]
                        self.store_facade.add_product_amount(store_id, product_id, amount)
            if purchase_accepted:
                self.purchase_facade.reject_purchase(pur_id)
            if basket_cleared:
                self.user_facade.restore_basket(user_id, cart)
            raise e

    def nominate_store_owner(self, store_id: int, owner_id: int, new_owner_id: int):
        nomination_id = self.roles_facade.nominate_owner(store_id, owner_id, new_owner_id)
        # TODO: different implementation later
        self.user_facade.notify_user(new_owner_id,
                                     NotificationDTO(-1, f"You have been nominated to be the owner of store"
                                                         f" {store_id}. nomination id: {nomination_id} ",
                                                     datetime.now()))

    def nominate_store_manager(self, store_id: int, owner_id: int, new_manager_id: int):
        nomination_id = self.roles_facade.nominate_manager(store_id, owner_id, new_manager_id)
        # TODO: different implementation later
        self.user_facade.notify_user(new_manager_id,
                                     NotificationDTO(-1, f"You have been nominated to be the manager of store"
                                                         f" {store_id}. nomination id: {nomination_id} ",
                                                     datetime.now()))

    def accept_nomination(self, user_id: int, nomination_id: int, accept: bool):
        if accept:
            self.roles_facade.accept_nomination(nomination_id, user_id)
        else:
            self.roles_facade.decline_nomination(nomination_id, user_id)

    def change_permissions(self, actor_id: int, store_id: int, manager_id: int, add_product: bool,
                           change_purchase_policy: bool, change_purchase_types: bool, change_discount_policy: bool,
                           change_discount_types: bool, add_manager: bool, get_bid: bool):
        self.roles_facade.set_manager_permissions(store_id, actor_id, manager_id, add_product, change_purchase_policy,
                                                  change_purchase_types, change_discount_policy, change_discount_types,
                                                  add_manager, get_bid)

    def add_system_manager(self, actor: int, user_id: int):
        self.roles_facade.add_system_manager(actor, user_id)

    def remove_system_manager(self, actor: int, user_id: int):
        self.roles_facade.remove_system_manager(actor, user_id)

    def add_payment_method(self, user_id: int, method_name: str, payment_config: Dict):
        if not self.roles_facade.is_system_manager(user_id):
            raise ValueError("User is not a system manager")
        PaymentHandler().add_payment_method(method_name, payment_config)

    def edit_payment_method(self, user_id: int, method_name: str, editing_data: Dict):
        if not self.roles_facade.is_system_manager(user_id):
            raise ValueError("User is not a system manager")
        PaymentHandler().edit_payment_method(method_name, editing_data)

    def remove_payment_method(self, user_id: int, method_name: str):
        if not self.roles_facade.is_system_manager(user_id):
            raise ValueError("User is not a system manager")
        PaymentHandler().remove_payment_method(method_name)

    def add_supply_method(self, user_id: int, method_name: str, supply_config: Dict):
        if not self.roles_facade.is_system_manager(user_id):
            raise ValueError("User is not a system manager")
        SupplyHandler().add_supply_method(method_name, supply_config)

    def edit_supply_method(self, user_id: int, method_name: str, editing_data: Dict):
        if not self.roles_facade.is_system_manager(user_id):
            raise ValueError("User is not a system manager")
        SupplyHandler().edit_supply_method(method_name, editing_data)

    def remove_supply_method(self, user_id: int, method_name: str):
        if not self.roles_facade.is_system_manager(user_id):
            raise ValueError("User is not a system manager")
        SupplyHandler().remove_supply_method(method_name)

    # -------------------------------------- Store Related Methods --------------------------------------#
    def search_by_category(self, category_id: int, sort_type: int) -> List[Tuple[Tuple[int, int], Tuple[float, float]]]:
        """
        * Parameters: categoryId, sortByLowesToHighestPrice
        * This function returns the list of all productIds
        * Note: if sortType is 1, the list will be sorted by lowest to highest price, 2 is highest to lowest, 3 is by
        rating lowest to Highest, 4 is by highest to lowest
        * Returns a list of product ids of the products in the category with categoryId
        """
        product_specifications_of_category = self.store_facade.get_product_spec_of_category(category_id)
        product_ids_to_store: List[Tuple[Tuple[int, int], Tuple[float, float]]] = []
        # TODO: types doesnt match
        """for store in self.store_facade.stores:
            for product in store.store_products:
                if product.specification_id in product_specifications_of_category:
                    product_ids_to_store.append((product.product_id, product.specification_id, store.store_id,
                                              store.store_name, product.price,
                                              store.ratings_of_product_spec_id[product.specification_id]))"""
        if sort_type == 1:
            product_ids_to_store.sort(key=lambda x: x[1][0])
        elif sort_type == 2:
            product_ids_to_store.sort(key=lambda x: x[1][0], reverse=True)
        elif sort_type == 3:
            product_ids_to_store.sort(key=lambda x: x[1][1])
        elif sort_type == 4:
            product_ids_to_store.sort(key=lambda x: x[1][1], reverse=True)
        return product_ids_to_store

    def search_by_tags(self, tags: List[str], sort_type: int) -> List[Tuple[Tuple[int, int], Tuple[float, float]]]:
        """
        * Parameters: tags, sortByLowesToHighestPrice
        * This function returns the list of all productIds
        * Note: if sortType is 1, the list will be sorted by lowest to highest price, 2 is highest to lowest, 3 is by
         rating lowest to Highest, 4 is by highest to lowest
        * Returns a list of product ids of the products with the tags in tags
        """
        product_specifications_of_tags = self.store_facade.get_product_specs_by_tags(tags)
        product_ids_to_store: List[Tuple[Tuple[int, int], Tuple[float, float]]] = []
        # TODO: types doesnt match
        """for store in self.store_facade.stores:
            for product in store.store_products:
                if product.specification_id in product_specifications_of_tags:
                    product_ids_to_store.append((product.product_id, product.specification_id, store.store_id, 
                                                 store.store_name, product.price, 
                                                 store.ratings_of_product_spec_id[product.specification_id]))"""

        if sort_type == 1:
            product_ids_to_store.sort(key=lambda x: x[1][0])
        elif sort_type == 2:
            product_ids_to_store.sort(key=lambda x: x[1][0], reverse=True)
        elif sort_type == 3:
            product_ids_to_store.sort(key=lambda x: x[1][1])
        elif sort_type == 4:
            product_ids_to_store.sort(key=lambda x: x[1][1], reverse=True)
        return product_ids_to_store

    def search_by_names(self, name: str, sort_type: int) -> List[Tuple[Tuple[int, int], Tuple[float, float]]]:
        """
        * Parameters: names, sortByLowesToHighestPrice
        * This function returns the list of all productIds
        * Note: if sortType is 1, the list will be sorted by lowest to highest price, 2 is highest to lowest, 3 is by
         rating lowest to Highest, 4 is by highest to lowest
        * Returns a list of product ids of the products with the names in names
        """
        product_specifications_of_names = self.store_facade.get_product_spec_by_name(name)
        product_ids_to_store: List[Tuple[Tuple[int, int], Tuple[float, float]]] = []
        # TODO: types doesnt match
        """for store in self.store_facade.stores:
            for product in store.store_products:
                if product.specification_id in product_specifications_of_names:
                    product_ids_to_store.append((product.product_id, product.specification_id,
                                              store.store_id, store.store_name, product.price,
                                              store.ratings_of_product_spec_id[product.specification_id]))"""

        if sort_type == 1:
            product_ids_to_store.sort(key=lambda x: x[1][0])
        elif sort_type == 2:
            product_ids_to_store.sort(key=lambda x: x[1][0], reverse=True)
        elif sort_type == 3:
            product_ids_to_store.sort(key=lambda x: x[1][1])
        elif sort_type == 4:
            product_ids_to_store.sort(key=lambda x: x[1][1], reverse=True)
        return product_ids_to_store

    def search_product_in_store(self, store_id: int, name: str, sort_type: int) \
            -> List[Tuple[int, Tuple[float, float]]]:
        """
        * Parameters: storeId, names, sortByLowesToHighestPrice
        * This function returns the list of all productIds
        * Note: if sortType is 1, the list will be sorted by lowest to highest price, 2 is highest to lowest, 3 is by
        rating lowest to Highest, 4 is by highest to lowest
        * Returns a list of product ids of the products with the names in names
        """
        product_specifications_of_names = self.store_facade.get_product_spec_by_name(name)
        product_ids_to_store: List[Tuple[int, Tuple[float, float]]] = []
        store = self.store_facade.get_store_by_id(store_id)
        # TODO: types doesnt match
        """for product in store.store_products:
            if product.specification_id in product_specifications_of_names:
                product_ids_to_store.append((product.product_id, product.price,
                                          store.ratings_of_product_spec_id[product.specification_id]))"""

        if sort_type == 1:
            product_ids_to_store.sort(key=lambda x: x[1][0])
        elif sort_type == 2:
            product_ids_to_store.sort(key=lambda x: x[1][0], reverse=True)
        elif sort_type == 3:
            product_ids_to_store.sort(key=lambda x: x[1][1])
        elif sort_type == 4:
            product_ids_to_store.sort(key=lambda x: x[1][1], reverse=True)
        return product_ids_to_store

    def get_store_info(self, user_id: int, store_id: int) -> Optional[str]:
        """
        * Parameters: storeId
        * This function returns the store information
        * Returns the store information
        """
        # TODO: check if user has necessary permissions to view store information
        if self.store_facade.get_store_by_id(store_id) is not None:
            return self.store_facade.get_store_by_id(store_id).get_store_information()
        return None

    def get_store_product_info(self, user_id: int, store_id: int) -> str:
        """
        * Parameters: storeId
        * This function returns the store product information
        * Returns the store product information
        """
        # TODO: check if user has necessary permissions to view store product information
        return self.store_facade.get_store_product_information(user_id, store_id)

    # -------------Discount related methods-------------------#
    def add_discount(self, user_id: int, description: str, start_date: datetime, ending_date: datetime,
                     percentage: float):
        # later on we need to support the creation of different types of discounts using hasStoreId?: int etc, maybe
        # wildcards could be useful
        # TODO: check if user has necessary permissions to add a discount
        # if self.roles_facade.check_permissions(userId, "add_discount"):
        if self.store_facade.add_discount(description, start_date, ending_date, percentage):
            logger.info(f"User {user_id} has added a discount")
        else:
            logger.info(f"User {user_id} has failed to add a discount")

    def change_discount(self, user_id: int, discount_id: int):
        # TODO: not implemented yet, but supported partially in purchaseFacade, see changeDiscountPercentage for example
        pass

    def remove_discount(self, user_id: int, discount_id: int):
        # TODO: check if user has necessary permissions to remove a discount
        # TODO: check if what i did instead is ok:
        if not self.roles_facade.is_system_manager(user_id):
            raise ValueError("User is not a system manager")
        if self.store_facade.remove_discount(discount_id):
            logger.info(f"User {user_id} has removed a discount")
        else:
            logger.info(f"User {user_id} has failed to remove a discount")

    # -------------Rating related methods-------------------#
    '''def add_store_rating(self, user_id: int, purchase_id: int, description: str, rating: float):
        """
        * Parameters: user_id, purchase_id, description, rating
        * This function adds a rating to a store
        * Returns None
        """
        store_id = self.purchase_facade.get_purchase_by_id(purchase_id).store_id
        new_rating = self.purchase_facade.rate_store(purchase_id, user_id, store_id, rating, description)
        if new_rating is not None:
            self.store_facade.update_store_rating(store_id, new_rating)
            logger.info(f"User {user_id} has rated store {store_id} with {rating}")
        else:
            logger.info(f"User {user_id} has failed to rate store {store_id}")'''

    '''def add_product_rating(self, user_id: int, purchase_id: int, description: str, product_spec_id: int, rating: float):
        """
        * Parameters: user_id, purchase_id, description, productSpec_id, rating
        * This function adds a rating to a product
        * Returns None
        """
        store_id = self.purchase_facade.get_purchase_by_id(purchase_id).store_id
        new_rating = self.purchase_facade.rate_product(purchase_id, user_id, product_spec_id, rating, description)
        if new_rating is not None:
            self.store_facade.update_product_spec_rating(store_id, product_spec_id, new_rating)
            logger.info(f"User {user_id} has rated product {product_spec_id} with {rating}")
        else:
            logger.info(f"User {user_id} has failed to rate product {product_spec_id}")'''

    # -------------Policies related methods-------------------#
    def add_purchase_policy(self, user_id: int, store_id: int):
        # for now, we dont support the creation of different types of policies
        """
        * Parameters: userId, store_id
        * This function adds a purchase policy to the store
        * Returns None
        """
        if self.roles_facade.has_change_purchase_policy_permission(store_id, user_id):
            self.store_facade.add_purchase_policy_to_store(store_id)
        else:
            raise ValueError("User does not have the necessary permissions to add a policy to the store")

    def remove_purchase_policy(self, user_id, store_id: int, policy_id: int):
        """
        * Parameters: store_id, policy_id
        * This function removes a purchase policy from the store
        * Returns None
        """
        if self.roles_facade.has_change_purchase_policy_permission(store_id, user_id):
            self.store_facade.remove_purchase_policy_from_store(store_id, policy_id)
        else:
            raise ValueError("User does not have the necessary permissions to remove a policy from the store")

    def change_purchase_policy(self, user_id: int, store_id: int, policy_id: int):  # not implemented yet
        pass

    # -------------Products related methods-------------------#
    def add_product(self, user_id: int, store_id: int, product_spec_id: int, expiration_date: datetime, condition: int,
                    price: float):
        """
        * Parameters: user_id, store_id, productSpecId, expirationDate, condition, price
        * This function adds a product to the store
        * Returns None
        """
        if not self.roles_facade.has_add_product_permission(store_id, user_id):
            raise ValueError("User does not have the necessary permissions to add a product to the store")
        if self.store_facade.add_product_to_store(store_id, product_spec_id, expiration_date, condition, price):
            logger.info(f"User {user_id} has added a product to store {store_id}")
        else:
            logger.info(f"User {user_id} has failed to add a product to store {store_id}")

    def remove_product(self, user_id: int, store_id: int, product_id: int):
        """
        * Parameters: store_id, product_id
        * This function removes a product from the store
        * Returns None
        """
        if not self.roles_facade.has_add_product_permission(store_id, user_id):
            raise ValueError("User does not have the necessary permissions to remove a product from the store")
        if self.store_facade.remove_product_from_store(store_id, product_id):
            logger.info(f"User {user_id} has removed a product from store {store_id}")
        else:
            logger.info(f"User {user_id} has failed to remove a product from store {store_id}")

    def change_product_price(self, user_id: int, store_id: int, product_id: int, new_price: float):
        """
        * Parameters: userId, store_id, product_id, newPrice
        * This function changes the price of a product
        * Returns None
        """
        if not self.roles_facade.has_add_product_permission(store_id, user_id):
            raise ValueError(
                "User does not have the necessary permissions to change the price of a product in the store")
        if self.store_facade.change_price_of_product(store_id, product_id, new_price):
            logger.info(f"User {user_id} has changed the price of product {product_id} in store {store_id}")
        else:
            logger.info(f"User {user_id} has failed to change the price of product {product_id} in store {store_id}")

    # -------------Store related methods-------------------#
    def add_store(self, founder_id: int, location_id: int, store_name: str):
        """
        * Parameters: founderId, locationId, storeName
        * This function adds a store to the system
        * Returns None
        """
        if not self.user_facade.is_member(founder_id):
            raise ValueError("User is not a member")
        self.store_facade.add_store(location_id, store_name, founder_id)

    def close_store(self, user_id: int, store_id: int):
        """
        * Parameters: userId, store_id
        * This function closes a store
        * Returns None
        """
        if not self.auth_facade.is_logged_in(user_id):
            raise ValueError("User is not logged in")
        if self.store_facade.close_store(store_id, user_id):
            logger.info(f"User {user_id} has closed store {store_id}")
        else:
            logger.info(f"User {user_id} has failed to close store {store_id}")

    # -------------Tags related methods-------------------#
    def add_tag_to_product_spec(self, user_id: int, product_spec_id: int, tag: str):
        """
        * Parameters: userId, productSpecId, tag
        * This function adds a tag to a product specification
        * Returns None
        """
        if not self.roles_facade.is_system_manager(user_id):
            raise ValueError("User is not a system manager")
        if self.store_facade.add_tag_to_product_specification(product_spec_id, tag):
            logger.info(f"User {user_id} has added a tag to product specification {product_spec_id}")
        else:
            logger.info(f"User {user_id} has failed to add a tag to product specification {product_spec_id}")

    def remove_tag_from_product_spec(self, user_id: int, product_spec_id: int, tag: str):
        """
        * Parameters: userId, productSpecId, tag
        * This function removes a tag from a product specification
        * Returns None
        """
        if not self.roles_facade.is_system_manager(user_id):
            raise ValueError("User is not a system manager")
        if self.store_facade.remove_tags_from_product_specification(product_spec_id, tag):
            logger.info(f"User {user_id} has removed a tag from product specification {product_spec_id}")
        else:
            logger.info(f"User {user_id} has failed to remove a tag from product specification {product_spec_id}")

    # -------------ProductSpec related methods-------------------#
    def change_product_spec_name(self, user_id: int, product_spec_id: int, new_name: str):
        """
        * Parameters: userId, productSpecId, newName
        * This function changes the name of a product specification
        * Returns None
        """
        if not self.roles_facade.is_system_manager(user_id):
            raise ValueError("User is not a system manager")
        if self.store_facade.change_name_of_product_specification(product_spec_id, new_name):
            logger.info(f"User {user_id} has changed the name of product specification {product_spec_id}")
        else:
            logger.info(f"User {user_id} has failed to change the name of product specification {product_spec_id}")

    def change_product_spec_manufacturer(self, user_id: int, product_spec_id: int, manufacturer: str):
        """
        * Parameters: userId, productSpecId, manufacturer
        * This function changes the manufacturer of a product specification
        * Returns None
        """
        if not self.roles_facade.is_system_manager(user_id):
            raise ValueError("User is not a system manager")
        if self.store_facade.change_manufacturer_of_product_specification(product_spec_id, manufacturer):
            logger.info(f"User {user_id} has changed the manufacturer of product specification {product_spec_id}")
        else:
            logger.info(f"User {user_id} has failed to change the manufacturer of product specification"
                        f" {product_spec_id}")

    def change_product_spec_description(self, user_id: int, product_spec_id: int, description: str):
        """
        * Parameters: userId, productSpecId, description
        * This function changes the description of a product specification
        * Returns None
        """
        if not self.roles_facade.is_system_manager(user_id):
            raise ValueError("User is not a system manager")
        if self.store_facade.change_description_of_product_specification(product_spec_id, description):
            logger.info(f"User {user_id} has changed the description of product specification {product_spec_id}")
        else:
            logger.info(f"User {user_id} has failed to change the description of product "
                        f"specification {product_spec_id}")

    def change_product_spec_weight(self, user_id: int, product_spec_id: int, weight_in_kilos: float):
        """
        * Parameters: userId, productSpecId, weightInKilos
        * This function changes the weight of a product specification
        * Returns None
        """
        if not self.roles_facade.is_system_manager(user_id):
            raise ValueError("User is not a system manager")
        if self.store_facade.change_weight_of_product_specification(product_spec_id, weight_in_kilos):
            logger.info(f"User {user_id} has changed the weight of product specification {product_spec_id}")
        else:
            logger.info(f"User {user_id} has failed to change the weight of product specification {product_spec_id}")

    # -------------Category related methods-------------------#
    def add_category(self, user_id: int, category_name: str):
        """
        * Parameters: userId, categoryName
        * This function adds a category to the system
        * Returns None
        """
        if not self.roles_facade.is_system_manager(user_id):
            raise ValueError("User is not a system manager")
        if self.store_facade.add_category(category_name):
            logger.info(f"User {user_id} has added a category")
        else:
            logger.info(f"User {user_id} has failed to add a category")

    def remove_category(self, user_id: int, category_id: int):
        """
        * Parameters: userId, categoryId
        * This function removes a category from the system
        * Returns None
        """
        if not self.roles_facade.is_system_manager(user_id):
            raise ValueError("User is not a system manager")
        if self.store_facade.remove_category(category_id):
            logger.info(f"User {user_id} has removed a category")
        else:
            logger.info(f"User {user_id} has failed to remove a category")

    def add_sub_category_to_category(self, user_id: int, sub_category_id: int, parent_category_id: int):
        """
        * Parameters: userId, subCategoryId, parentCategoryId
        * This function adds a sub category to a category
        * NOTE: It is assumed that the subCategory is already created and exists in the system
        * Returns None
        """
        if not self.roles_facade.is_system_manager(user_id):
            raise ValueError("User is not a system manager")
        if self.store_facade.assign_sub_category_to_category(sub_category_id, parent_category_id):
            logger.info(f"User {user_id} has added a sub category to category {parent_category_id}")
        else:
            logger.info(f"User {user_id} has failed to add a sub category to category {parent_category_id}")

    def remove_sub_category_from_category(self, user_id: int, category_id: int, sub_category_id: int):
        """
        * Parameters: userId, categoryId, subCategoryId
        * This function removes a sub category from a category
        * Returns None
        """
        if not self.roles_facade.is_system_manager(user_id):
            raise ValueError("User is not a system manager")
        if self.store_facade.delete_sub_category_from_category(category_id, sub_category_id):
            logger.info(f"User {user_id} has removed a sub category from category {category_id}")
        else:
            logger.info(f"User {user_id} has failed to remove a sub category from category {category_id}")

    def assign_product_spec_to_category(self, user_id: int, category_id: int, product_spec_id: int):
        """
        * Parameters: userId, categoryId, productSpecId
        * This function assigns a product specification to a category
        * NOTE: it is assumed that the product specification exists in the system
        * Returns None
        """
        if not self.roles_facade.is_system_manager(user_id):
            raise ValueError("User is not a system manager")
        if self.store_facade.assign_product_spec_to_category(category_id, product_spec_id):
            logger.info(f"User {user_id} has assigned a product specification to category {category_id}")
        else:
            logger.info(f"User {user_id} has failed to assign a product specification to category {category_id}")

    def remove_product_spec_from_category(self, user_id: int, category_id: int, product_spec_id: int):
        """
        * Parameters: userId, categoryId, productSpecId
        * This function removes a product specification from a category
        * Returns None
        """
        if not self.roles_facade.is_system_manager(user_id):
            raise ValueError("User is not a system manager")
        if self.store_facade.remove_product_spec_from_category(category_id, product_spec_id):
            logger.info(f"User {user_id} has removed a product specification from category {category_id}")
        else:
            logger.info(f"User {user_id} has failed to remove a product specification from category {category_id}")

    # -------------PurchaseFacade methods:-------------------#

    # -------------Purchase management related methods-------------------#

    '''def create_bid_purchase(self, user_id: int, proposed_price: float, product_id: int, store_id: int):
        """
        * Parameters: userId, proposedPrice, productId, storeId
        * This function creates a bid purchase
        * Returns None
        """
        if not self.user_facade.is_member(user_id) or not self.auth_facade.is_logged_in(user_id):
            raise ValueError("User is not a member or is not logged in")
        product = self.store_facade.get_store_by_id(store_id).get_product_by_id(product_id)
        product_spec_id = product.specification_id

        if self.purchase_facade.create_bid_purchase(user_id, proposed_price, product_id, product_spec_id, store_id):
            logger.info(f"User {user_id} has created a bid purchase")
            self.notifier.notify_new_bid(store_id, user_id)  # Notify each listener of the store about the bid
            # TODO: await for their reaction and handle them
        else:
            logger.info(f"User {user_id} has failed to create a bid purchase")

    def create_auction_purchase(self, user_id: int, base_price: float, starting_date: datetime, ending_date: datetime,
                                store_id: int, product_id: int):
        """
        * Parameters: userId, basePrice, startingDate, endingDate, productId, storeId
        * This function creates an auction purchase
        * Returns None
        """
        if not self.user_facade.is_member(user_id) or not self.auth_facade.is_logged_in(user_id):
            raise ValueError("User is not a member or is not logged in")
        product = self.store_facade.get_store_by_id(store_id).get_product_by_id(product_id)
        product_spec_id = product.specification_id
        if self.purchase_facade.create_auction_purchase(base_price, starting_date, ending_date, store_id, product_id,
                                                        product_spec_id):
            logger.info(f"User {user_id} has created an auction purchase")

    def create_lottery_purchase(self, user_id: int, full_price: float, store_id: int, product_id: int,
                                starting_date: datetime, ending_date: datetime):
        """
        * Parameters: userId, fullPrice, storeId, productId, startingDate, endingDate
        * This function creates a lottery purchase
        * Returns None
        """
        if not self.user_facade.is_member(user_id) or not self.auth_facade.is_logged_in(user_id):
            raise ValueError("User is not a member or is not logged in")
        product = self.store_facade.get_store_by_id(store_id).get_product_by_id(product_id)
        product_spec_id = product.specification_id
        if self.purchase_facade.create_lottery_purchase(user_id, full_price, store_id, product_id, product_spec_id,
                                                        starting_date, ending_date):
            logger.info(f"User {user_id} has created a lottery purchase")
        else:
            logger.info(f"User {user_id} has failed to create a lottery purchase")'''

    def view_purchases_of_user(self, user_id: int) -> List[PurchaseDTO]:
        """
        * Parameters: user_id
        * This function returns the purchases of a user
        * Returns a string
        """
        if not self.auth_facade.is_logged_in(user_id):
            raise ValueError("User is not logged in")
        return self.purchase_facade.get_purchases_of_user(user_id)

    def view_purchases_of_store(self, user_id: int, store_id: int) -> List[PurchaseDTO]:
        """
        * Parameters: userId, store_id
        * This function returns the purchases of a store
        * Returns a string
        """
        if not self.auth_facade.is_logged_in(user_id):
            raise ValueError("User is not a member or is not logged in")
        # TODO: add check if user is system manager or store owner or store manager, if not raise error
        return self.purchase_facade.get_purchases_of_store(store_id)

    '''def view_purchases_of_user_in_store(self, user_id: int, store_id: int) -> str:
        """
        * Parameters: userId, store_id
        * This function returns the purchases of a user in a store
        * Returns a string
        """
        if not self.user_facade.is_member(user_id) or not self.auth_facade.is_logged_in(user_id):
            raise ValueError("User is not a member or is not logged in")
        purchases = self.purchase_facade.get_purchases_of_user(user_id)
        str_output = ""
        for purchase in purchases:
            if purchase.store_id == store_id:
                str_output += purchase.__str__()
        return str_output

    def view_on_going_purchases(self, user_id: int) -> str:
        """
        * Parameters: userId
        * This function returns the ongoing purchases
        * Returns a string
        """
        if not self.user_facade.is_member(user_id) or not self.auth_facade.is_logged_in(user_id):
            raise ValueError("User is not a member or is not logged in")
        purchases = self.purchase_facade.get_on_going_purchases()
        str_output = ""
        for purchase in purchases:
            str_output += purchase.__str__()
        return str_output

    def view_completed_purchases(self, user_id: int) -> str:
        """
        * Parameters: userId
        * This function returns the completed purchases
        * Returns a string
        """
        if not self.user_facade.is_member(user_id) or not self.auth_facade.is_logged_in(user_id):
            raise ValueError("User is not a member or is not logged in")
        purchases = self.purchase_facade.get_completed_purchases()
        str_output = ""
        for purchase in purchases:
            str_output += purchase.__str__()
        return str_output

    def view_failed_purchases(self, user_id: int) -> str:
        """
        * Parameters: userId
        * This function returns the failed purchases
        * Returns a string
        """
        if not self.user_facade.is_member(user_id) or not self.auth_facade.is_logged_in(user_id):
            raise ValueError("User is not a member or is not logged in")
        purchases = self.purchase_facade.get_failed_purchases()
        str_output = ""
        for purchase in purchases:
            str_output += purchase.__str__()
        return str_output

    def view_accepted_purchases(self, user_id: int) -> str:
        """
        * Parameters: userId
        * This function returns the accepted purchases
        * Returns a string
        """
        if not self.user_facade.is_member(user_id) or not self.auth_facade.is_logged_in(user_id):
            raise ValueError("User is not a member or is not logged in")
        purchases = self.purchase_facade.get_accepted_purchases()
        str_output = ""
        for purchase in purchases:
            str_output += purchase.__str__()
        return str_output

    def handle_accepted_purchases(self):
        """
        * Parameters: None
        * This function handles the accepted purchases
        * Returns None
        """
        accepted_purchases = self.purchase_facade.get_accepted_purchases()
        for purchase in accepted_purchases:
            if self.purchase_facade.check_if_completed_purchase(purchase.purchase_id):
                logger.info(f"Purchase {purchase.purchase_id} has been completed")'''

    # -------------Bid Purchase related methods-------------------#
    '''def store_accept_offer(self, purchase_id: int):
        pass  # cant be implemented yet without notifications

    def store_reject_offer(self, purchase_id: int):
        pass  # cant be implemented yet without notifications

    def store_counter_offer(self, new_price: float, purchase_id: int):
        pass  # cant be implemented yet without notifications

    def user_accept_offer(self, user_id: int, purchase_id: int):  # TODO
        """
        * Parameters: userId, purchase_id
        * This function accepts an offer
        * Returns None
        """
        if not self.auth_facade.is_logged_in(user_id):
            raise ValueError("User is not logged in")
        self.purchase_facade.user_accept_offer(purchase_id, user_id)

        # notify the store owners and all relevant parties
        store_id = self.purchase_facade.get_purchase_by_id(purchase_id).store_id
        # Notify each listener of the store about the bid
        self.notifier.notify_general_listeners(store_id,
                                               f"User {user_id} has accepted the offer in purchase {purchase_id}")

    def user_reject_offer(self, user_id: int, purchase_id: int):
        """
        * Parameters: userId, purchase_id
        * This function rejects an offer
        * Returns None
        """
        if not self.auth_facade.is_logged_in(user_id):
            raise ValueError("User is not logged in")
        self.purchase_facade.user_reject_offer(purchase_id, user_id)

        # notify the store owners and all relevant parties
        msg = f"User {user_id} has rejected the offer in purchase {purchase_id}"
        store_id = self.purchase_facade.get_purchase_by_id(purchase_id).store_id
        self.notifier.notify_general_listeners(store_id, msg)  # Notify each listener of the store about the bid

    def user_counter_offer(self, user_id: int, counter_offer: float, purchase_id: int):
        """
        * Parameters: userId, counterOffer, purchase_id
        * This function makes a counter offer
        * Returns None
        """
        if not self.auth_facade.is_logged_in(user_id):
            raise ValueError("User is not logged in")
        self.purchase_facade.user_counter_offer(counter_offer, purchase_id)

        # notify the store owners and all relevant parties
        store_id = self.purchase_facade.get_purchase_by_id(purchase_id).store_id
        msg = f"User {user_id} has made a counter offer in purchase {purchase_id}"
        self.notifier.notify_general_listeners(store_id, msg)  # Notify each listener of the store about the bid'''

    # -------------Auction Purchase related methods-------------------#
    '''def add_auction_bid(self, purchase_id: int, user_id: int, price: float):
        """
        * Parameters: purchase_id, user_id, price
        * This function adds a bid to an auction purchase
        * Returns None
        """
        if not self.auth_facade.is_logged_in(user_id):
            raise ValueError("User is not logged in")
        if self.purchase_facade.add_auction_bid(user_id, price, purchase_id):
            logger.info(f"User {user_id} has added a bid to purchase {purchase_id}")

            # notify the store owners and all relevant parties
            msg = f"User {user_id} has added a bid to purchase {purchase_id}"
            store_id = self.purchase_facade.get_purchase_by_id(purchase_id).store_id
            self.notifier.notify_general_listeners(store_id, msg)  # Notify each listener of the store about the bid
        else:
            logger.info(f"User {user_id} has failed to add a bid to purchase {purchase_id}")

    def view_highest_bid(self, purchase_id: int, user_id: int) -> float:
        """
        * Parameters: purchase_id, userId
        * This function returns the highest bid of an auction purchase
        * Returns a float
        """
        if not self.auth_facade.is_logged_in(user_id):
            raise ValueError("User is not logged in")

        # notify the store owners and all relevant parties
        store_id = self.purchase_facade.get_purchase_by_id(purchase_id).store_id
        msg = f"User {user_id} has viewed the highest bid in purchase {purchase_id}"
        self.notifier.notify_general_listeners(store_id, msg)  # Notify each listener of the store about the bid

        return self.purchase_facade.view_highest_bidding_offer(purchase_id)

    def calculate_remaining_time_of_auction(self, purchase_id: int, user_id: int) -> timedelta:
        """
        * Parameters: purchase_id, userId
        * This function calculates the remaining time of an auction purchase
        * Returns a float
        """
        if not self.auth_facade.is_logged_in(user_id):
            raise ValueError("User is not logged in")

        # TODO: notify the store owners and all relevant parties
        # NOTE: I did it, but why?
        store_id = self.purchase_facade.get_purchase_by_id(purchase_id).store_id
        msg = f"User {user_id} has calculated the remaining time of auction {purchase_id}"
        self.notifier.notify_general_listeners(store_id, msg)  # Notify each listener of the store about the bid

        return self.purchase_facade.calculate_remaining_time(purchase_id)

    def handle_ongoing_auctions(self):
        """
        * Parameters: None
        * This function handles the ongoing auctions
        * Returns None
        """
        ongoing_purchases = self.purchase_facade.get_on_going_purchases()
        for purchase in ongoing_purchases:
            # NOTE: what does "2" mean?, maybe we should use magic numbers
            # TODO: getPurchaseType does not exist in Purchase, should be added
            """if purchase.getPurchaseType(purchase.purchase_id()) == 2:
                if self.purchase_facade.check_if_auction_ended(purchase.purchase_id()):
                    #TODO: notify the store owners and all relevant parties

                    #TODO: notify the user who won the auction
                    #NOTE: How do we know who won the auction? 

                    #TODO: third party services
                    #if thirdparty services work, then:
                    #TODO: call validatePurchaseOfUser(purchase.get_purchaseId(), purchase.get_userId(), deliveryDate)
                    #else:
                    #TODO: call invalidatePurchase(purchase.get_purchaseId(), purchase.get_userId()
                    logger.info(f"Auction {purchase.purchase_id()} has been completed")"""'''

    # -------------Lottery Purchase related methods-------------------#
    '''def add_lottery_offer(self, user_id: int, proposed_price: float, purchase_id: int):
        """
        * Parameters: user_id, proposedPrice, purchase_id
        * This function adds a lottery ticket to a lottery purchase
        * Returns None
        """

        if not self.auth_facade.is_logged_in(user_id) or not self.user_facade.is_member(user_id):
            raise ValueError("User is not logged in or is not a member")

        if self.purchase_facade.add_lottery_offer(user_id, proposed_price, purchase_id):
            logger.info(f"User {user_id} has added a lottery ticket to purchase {purchase_id}")

            # notify the store owners and all relevant parties
            store_id = self.purchase_facade.get_purchase_by_id(purchase_id).store_id
            msg = f"User {user_id} has added a lottery ticket to purchase {purchase_id}"
            self.notifier.notify_general_listeners(store_id, msg)  # Notify each listener of the store about the bid

        else:
            logger.info(f"User {user_id} has failed to add a lottery ticket to purchase {purchase_id}")

    def calculate_remaining_time_of_lottery(self, purchase_id: int, user_id: int) -> timedelta:
        """
        * Parameters: purchase_id, userId
        * This function calculates the remaining time of a lottery purchase
        * Returns a float
        """
        if not self.auth_facade.is_logged_in(user_id):
            raise ValueError("User is not logged in")

        remaining_time = self.purchase_facade.calculate_remaining_time(purchase_id)

        # notify the store owners and all relevant parties
        msg = f"The remaining time of lottery {purchase_id} is {remaining_time} of user {user_id}"
        store_id = self.purchase_facade.get_purchase_by_id(purchase_id).store_id
        self.notifier.notify_general_listeners(store_id, msg)  # Notify each listener of the store about the bid

        return remaining_time

    def calculate_probability_of_user(self, purchase_id: int, user_id: int) -> float:
        """
        * Parameters: purchase_id, user_id
        * This function calculates the probability of a user in a lottery purchase
        * Returns a float
        """
        if not self.auth_facade.is_logged_in(user_id):
            raise ValueError("User is not logged in")

            # NOTE: I did it, but do we really need to notify the store owners and all relevant parties?
        # notify the store owners and all relevant parties

        probability = self.purchase_facade.calculate_probability_of_user(purchase_id, user_id)
        store_id = self.purchase_facade.get_purchase_by_id(purchase_id).store_id

        msg = f"The probability of user {user_id} in lottery {purchase_id} is {probability}"
        self.notifier.notify_general_listeners(store_id, msg)
        self.notifier.notify_general_message(user_id, msg)

        return probability

    def handle_ongoing_lotteries(self):
        """
        * Parameters: None
        * This function handles the ongoing lotteries
        * Returns None
        """
        ongoing_purchases = self.purchase_facade.get_on_going_purchases()
        for purchase in ongoing_purchases:
            # TODO: getPurchaseType does not exist in Purchase, should be added
            """if purchase.getPurchaseType(purchase.purchase_id()) == 3:
                if self.purchase_facade.validate_user_offers(purchase.purchase_id()):
                    userIdOfWinner = self.purchase_facade.pick_winner(purchase.purchase_id())
                    if userIdOfWinner is not None:
                        #notify the user who won the lottery
                        msg = "You have won the lottery on purchase" + purchase.purchase_id()
                        Notifier.notify_general_message(userIdOfWinner, msg)

                        #TODO: third party services
                        #if thirdparty services work, then:
                        #TODO: call validateDeliveryOfWinner(purchase.get_purchaseId(), purchase.get_userId(), deliveryDate)
                        #else:
                        #TODO: call invali  dateDeliveryOfWinner(purchase.get_purchaseId(), purchase.get_userId()
                        # logger.info(f"Lottery {purchase.get_purchaseId()} has been won!")
                    else:
                        #TODO: refund users who participated in the lottery
                        logger.info(f"Lottery {purchase.purchase_id()} has failed! Refunded all participants")"""'''
