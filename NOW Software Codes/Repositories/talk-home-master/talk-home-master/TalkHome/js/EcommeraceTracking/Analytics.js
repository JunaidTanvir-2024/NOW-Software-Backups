var CheckoutStep1Bit = false;
var CheckoutStep2Bit = false;
var CheckoutStep3Bit = false;
var CheckoutStep4Bit = false;

$(".ProductDetail").click(function () {
    //var button = $('.ProductDetail');//this.parentElement.parentElement.nextElementSibling.childNodes.item(9);
    impressionsobj = {
        'name': this.dataset.name,       // Name or ID is required.
        'id': this.dataset.id,
        'brand': 'TalkHome',
        'category': 'sim Cards',
        'variant': this.dataset.name,
        'list': "Plans",
        'position': 1,
        'url': window.location.pathname
    }
    ProductClick(impressionsobj)
})


$("#aPay360PayWithPaypal ,#new-card,#inputPay360NameOnCard,#inputPay360NameOnCard,#inputPay360CardNumber,#inputPay360SecurityCodeNewCard").click(() => {
    if (!CheckoutStep4Bit)
        CheckoutStep4();
    CheckoutStep4Bit = true;
})


$(".StartPayment,.StartPaymentTopUp").click(function () {
    var option = $('#HiddenPaymentMethod').val();
    CheckoutStep5(option);
    console.log(this.dataset);
    Purchase(this.dataset);
})


//Top uP Step !

//$("[name='(THM)-amount']").click((e) => {
//    if (!CheckoutStep1Bit) {
//        var checkoutstep1Obj = {
//            id: e.target.id,
//            name: e.target.name,
//            category: 'Top Up',
//            variant: e.target.name,
//            brand: 'TalkHome',
//            quantity: 1,
//            dimension3: 'Ecommerce',
//            metric5: 12,
//            metric6: 1002
//        }
//        CheckoutStep1(checkoutstep1Obj);
//    }
//    CheckoutStep1Bit = true;
//})