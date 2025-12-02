using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace telegram_Payment
{
    internal class create_order
    {
        import requests

def create_order(api_key, gateway, plan):
    if plan == 'basicplan':
        value = 599
    elif plan == 'goldplan':
        value = 999
    elif plan == 'diamondplan':
        value = 1499
    endpoint = 'https://dev.sellix.io/v1/payments'
    payload = {
        "title": "License Key",
        "value": value,
        "currency": "USD",
        "quantity": 1,
        "email": "marionfunny@desertsundesigns.com", #Enter your email here for logging purposes
        "gateway": gateway,
        "white_label": True,
        "confirmations" : 2 #change it if needed
    }
    headers = {
        'Authorization': f'Bearer {api_key}',
        'Content-Type': 'application/json'
    }
response = requests.post(endpoint, json=payload, headers=headers)
    
    if response.status_code == 200:
        data = response.json()
        
        # Check if all expected keys are in the response
        if data and isinstance(data.get('data'), dict) and isinstance(data['data'].get('invoice'), dict):
            invoice = data['data']['invoice']
            crypto_uri = invoice.get('crypto_uri')
            uniqid = invoice.get('uniqid')
            
            if crypto_uri and uniqid:
                try:
                    address, amount_param = crypto_uri.split('?')
                    # Insert a space after the colon in the address
                    if ':' in address:
                        protocol, addr = address.split(':', 1)
                        address = addr
                    amount = amount_param.split('=')[1]
                    return address, amount, uniqid, protocol, value
                except Exception as e:
                    print("Error splitting crypto_uri:", crypto_uri)
                    raise Exception(f"Error processing crypto_uri: {e}")
            else:
                print("crypto_uri or uniqid is missing:", invoice)
                raise Exception("Missing crypto_uri or uniqid in the response.")
        else:
            print("Unexpected response structure:", data)
            raise Exception("Unexpected data structure in response.")
    else:
        print(f"Failed to create payment. Status Code: {response.status_code}, Response: {response.text}")
        raise Exception(f"Failed to create payment. Status Code: {response.status_code}, Response: {response.text}")
import requests

def create_order(api_key, gateway, plan):
    if plan == 'basicplan':
        value = 599
    elif plan == 'goldplan':
        value = 999
    elif plan == 'diamondplan':
        value = 1499
    endpoint = 'https://dev.sellix.io/v1/payments'
    payload = {
        "title": "License Key",
        "value": value,
        "currency": "USD",
        "quantity": 1,
        "email": "marionfunny@desertsundesigns.com", #Enter your email here for logging purposes
        "gateway": gateway,
        "white_label": True,
        "confirmations" : 2 #change it if needed
    }
    headers = {
        'Authorization': f'Bearer {api_key}',
        'Content-Type': 'application/json'
    }
response = requests.post(endpoint, json=payload, headers=headers)
    
    if response.status_code == 200:
        data = response.json()
        
        # Check if all expected keys are in the response
        if data and isinstance(data.get('data'), dict) and isinstance(data['data'].get('invoice'), dict):
            invoice = data['data']['invoice']
            crypto_uri = invoice.get('crypto_uri')
            uniqid = invoice.get('uniqid')
            
            if crypto_uri and uniqid:
                try:
                    address, amount_param = crypto_uri.split('?')
                    # Insert a space after the colon in the address
                    if ':' in address:
                        protocol, addr = address.split(':', 1)
                        address = addr
                    amount = amount_param.split('=')[1]
                    return address, amount, uniqid, protocol, value
                except Exception as e:
                    print("Error splitting crypto_uri:", crypto_uri)
                    raise Exception(f"Error processing crypto_uri: {e}")
            else:
                print("crypto_uri or uniqid is missing:", invoice)
                raise Exception("Missing crypto_uri or uniqid in the response.")
        else:
            print("Unexpected response structure:", data)
            raise Exception("Unexpected data structure in response.")
    else:
        print(f"Failed to create payment. Status Code: {response.status_code}, Response: {response.text}")
        raise Exception(f"Failed to create payment. Status Code: {response.status_code}, Response: {response.text}")

    }
}
