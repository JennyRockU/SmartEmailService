# SmartEmailService
An application to send emails via API

## Description
The SmartEmailService project is a simple API which accepts the email details and sends the email, by unifiying different email service vendors. While this is largely a back-end project, the code in this repository also include a static page with the [API documentation](http://smartemailservice.azurewebsites.net).

## Architecture
The [SendEmailApi](../master/SendEmailApi) recieves POST calls with the relevant email details (recipients, subject etc.) and adds this message to a queue. Later, there are two seperate cloud application roles which read from this queue and try to handle the message. Each role uses the API of a different email provider ([SendGrid](https://sendgrid.com/docs/API_Reference/Web_API/mail.html), [MailGun](http://documentation.mailgun.com/quickstart.html#sending-messages)). If one service is down/fails to send the message, it is being returned back to the queue and then attempted again by another service.

Here is an overview of this desgin:

Email Message from User :arrow_down:

(sent to API)

SendEmailAPI (WebAPI) :arrow_down:
      
(sends the message to queue)

Email is processed by one of the vendors: :arrow_down:

- SendGrid
- MailGun

## Scalability
The fact that the different email vendors are running as seperate applications in the cloud, and barely share any code (except for the IMailSender interface), allows a lot of scaling flexibilty.

For example, once our SendEmailService grows larger and fills up the queue with a lot of email messages which await to be processed, it is very easy to add another application to help reduce the load and read from this queue.
All that is needed is to select another email service provider (e.g.: Amazon SES, Sparkpost) and deploy a role that is reading from the same queue, that's it.

It is also possible to increase the parallelism/role instance amount so that each (or just one) provider will run in parallel and process messages from the queue faster.

This design also, allows flexibity in the maintainces of each role. If for instance, SendGrid vendor requires a code change/fix, or if it goes down, the current architecture allows deploying/shutting off SendGrid's role individually. 

## Tradeoffs & Constraints


## Hosting

## Monitoring

## External Libraries

## Language Choices

