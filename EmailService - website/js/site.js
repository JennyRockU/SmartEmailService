﻿
// send the user form content to the EmailService API
async function SendEmail(recipient, sender, title, body) {

    const url = getApiUrl(recipient, sender, title);    
    
    try {
        var res = await fetch(url, {
            method: "post",
            mode: 'no-cors',
            body: body
        }).then(response => response)
        .catch(function(e) {
            return 'faild';
        })
        .then(function(response) {
            return 'success';
        });

        console.log(res);
        return res;

    } catch (exception){
        console.log('failed to send email');
    }
}

// build the full API url
function getApiUrl(recipient, sender, title) {

    return "http://smartsendemailapi.azurewebsites.net/api/emails/SendEmail/?" +
        `to=${recipient}&` +
        `from=${sender}&` +
        `subject=${title}`;
}

// send the user submitted content upon click on the 'send' button
$(document).ready(function () { 
    $('.form').on('submit', async function (event) {
    
        event.preventDefault();
        console.log('Form submitted!');
    
            try {
                // parse the user input
                let userInputs = document.getElementsByTagName('input');
                let from = userInputs[0].value;
                let to =  userInputs[1].value;
                let title = userInputs[2].value;
                let content = $('.textarea.input').val();
            
                // send to API
                var response = await SendEmail(to, from, title, content);

                // display feedback message
                if (response == "success") {
                    showSuccessMessage();
                } else {
                    showFailureMessage();
                }
                    
            } catch (exception) {
                console.log('failed');
            }
            
        });
     });

// quick link to the API documentation section
function goToDocumentation() {
    let docSection = $('#documenation');

    $(docSection.click(function(){
        docSection.scrollIntoView(true);
    }));
}

function showSuccessMessage(){

    $('.buttonWrapper')[0].style.display = "none";
    $('#success')[0].innerHTML = 'Thank you, your email is on its way!';
}

function showFailureMessage(){
    $('#failed')[0].innerHTML = 
                        'Sorry, something went wrong. ' +
                        'Please try again or contact our <a href="mailto:jennyrukman@gmail.com">support</a>.';

}