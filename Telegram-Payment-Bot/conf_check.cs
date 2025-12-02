import requests

def check_order_status(api_key, uniqid):
    url = f"https://dev.sellix.io/v1/orders/{uniqid}"
    headers = { "Authorization": f"Bearer {api_key}"}
status, crypto_hash = None, None

    try:
        response = requests.get(url, headers = headers)
        if response.status_code != 200:
            print(f"Non-successful response code: {response.status_code}")
            return status, crypto_hash

# Ensure the response contains valid JSON
        response_json = response.json()
        if not isinstance(response_json, dict):
            print(f"Invalid JSON response: {response_json}")
            return status, crypto_hash

# Extract order details
        order_details = response_json.get('data', {}).get('order', { })
        if not isinstance(order_details, dict):
            print(f"Invalid order details format: {order_details}")
            return status, crypto_hash

# Extract status and crypto hash
        status = order_details.get('status')
        if status in ["WAITING_FOR_CONFIRMATIONS", "COMPLETED"]:
            transactions = order_details.get('crypto_transactions', [])
            if transactions and isinstance(transactions, list):
                crypto_hash = transactions[0].get('hash')

    except requests.RequestException as e:
        print(f"Failed to get order status: {e}")

    return status, crypto_hash
s