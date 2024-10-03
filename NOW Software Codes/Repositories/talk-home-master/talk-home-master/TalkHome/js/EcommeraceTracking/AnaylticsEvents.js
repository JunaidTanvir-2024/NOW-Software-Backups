function ProductImpression(impressionArray) {
    var impressions = [];
    impressionArray.forEach((i) => {
        impressionsobj = {
            'name': i[0].name,       // Name or ID is required.
            'id': i[0].id,
            'brand': 'TalkHome',
            'category': i[0].category,
            'variant': i[0].variant,
            'list': i[0].list,
            'position': i[0].position
        }

        impressions.push(impressionsobj);
    })


    window.dataLayer = window.dataLayer || [];
    window.dataLayer.push({
        event: 'eec.impressionView',
        ecommerce: {
            impressions: impressions
        }
    });
}

function ProductClick(productObj) {
    window.dataLayer = window.dataLayer || [];
    window.dataLayer.push({
        event: 'eec.impressionClick',
        ecommerce: {
            click: {
                actionField: {
                    list: 'Related products'
                },
                products: [productObj]
            }
        }
    });


}

function CheckoutStep1(obj) {
    
    window.dataLayer = window.dataLayer || [];
    window.dataLayer.push({
        event: 'eec.checkout',
        ecommerce: {
            checkout: {
                actionField: {
                    step: 1
                },
                products: [
                    {
                    id: obj.id,
                    name: obj.name,
                    category: obj.category,
                    variant: obj.name,
                    brand: 'TalkHome',
                    quantity: 1,
                    dimension3: 'Ecommerce',
                    metric5: 12,
                    metric6: 1002
                }
                ]
            }
        }
    });

}

//function CheckoutStep2(obj) {
//    window.dataLayer = window.dataLayer || [];
//    window.dataLayer.push({
//        event: 'eec.checkout',
//        ecommerce: {
//            checkout: {
//                actionField: {
//                    step: 2,
//                    option: 'MasterCard'
//                }
//            }
//        }
//    });

//}

//function CheckoutStep3() {
//    window.dataLayer = window.dataLayer || [];
//    window.dataLayer.push({
//        event: 'eec.checkout',
//        ecommerce: {
//            checkout: {
//                actionField: {
//                    step: 3,
//                    option: 'MasterCard'
//                }
//            }
//        }
//    });
//}

function CheckoutStep4() {
    window.dataLayer = window.dataLayer || [];
    window.dataLayer.push({
        event: 'eec.checkout',
        ecommerce: {
            checkout: {
                actionField: {
                    step: 2,
                    option: 'MasterCard'
                }
            }
        }
    });
}

function CheckoutStep5(option) {
    window.dataLayer = window.dataLayer || [];
    window.dataLayer.push({
        event: 'eec.checkout',
        ecommerce: {
            checkout: {
                actionField: {
                    step: 3,
                    option: option
                }
            }
        }
    });
}

function Purchase(obj) {
    window.dataLayer = window.dataLayer || [];
    window.dataLayer.push({
        event: 'eec.purchase',
        ecommerce: {
            currencyCode: 'GBP',
            purchase: {
                actionField: {
                    id: obj.id,
                    affiliation: obj.name,
                    revenue: obj.price,
                    tax: '0',
                    shipping: '0',
                    coupon: 'SUMMER2019'
                },
                products: [obj]
            }
        }
    });
    console.log("Purchase success");
}

function loggedInUser(UserId) {
    window.dataLayer = window.dataLayer || [];
    window.dataLayer.push({
        'event': 'login',
        'userId': UserId // this number must be replaced with their mobile number or unique ID
    })
    console.log("login tracking success");
}


function TrackingOnEveryPageLoad(userId) {
    window.dataLayer = window.dataLayer || [];
    window.dataLayer.push({
        'userId': userId // this number must be replaced with their mobile number or unique ID
    })
    console.log("on every page load");
}