<p align="center">
  <a href="https://dotnet.microsoft.com/" target="blank"><img src="https://upload.wikimedia.org/wikipedia/commons/e/ee/.NET_Core_Logo.svg" width="120" alt=".NET Logo" /></a>
</p>
<body>
    <main>
        <section>
            <p>
                This API is responsible for managing user authentication securely and efficiently, using JWT (JSON Web Token) with asymmetric keys. 🔐  
                Additionally, it offers integration with the Customer API to ensure consistency in user creation across multiple systems. 
            </p>
        </section>

<section>
            <h2>Main Features</h2>
            <ul>
                <li>JWT token generation using asymmetric keys (public and private). 🔑</li>
                <li>Exposure of the <code>/jwks</code> endpoint for JWT token validation via the public key. 🛡️</li>
                <li>Secure storage of the private key in the database, with periodic rotation. 🔒</li>
                <li>Caching of the <code>/jwks</code> endpoint to optimize performance. 🚀</li>
                <li>Integration with the Customer API via RabbitMQ using the RPC pattern for data synchronization. 🔗</li>
                <li>Error handling and rollback during the user creation process. 🛠️</li>
            </ul>
        </section>

 <section>
            <h2>JWT Tokens and JWKS Architecture</h2>
            <p>
                The API uses the JWT (JSON Web Token) standard to authenticate and authorize users. This standard is widely adopted for its lightweight design and ease of use in distributed environments.
            </p>
            <h3>How JWT Works</h3>
            <ul>
                <li>A JWT consists of three parts:
                    <ol>
                        <li><strong>Header:</strong> Contains information about the token type and signing algorithm used (e.g., RS256). 🧾</li>
                        <li><strong>Payload:</strong> Includes user information (claims) such as ID, name, and permissions. 👤</li>
                        <li><strong>Signature:</strong> Generated using the private key, ensuring the token's integrity and authenticity. 🛡️</li>
                    </ol>
                </li>
                <li>When a JWT is generated, it is signed with the API's private key, ensuring that only the authentication API can issue valid tokens. 🔏</li>
                <li>Other APIs validate the JWT by consulting the public key exposed at the <code>/jwks</code> endpoint. 🔓</li>
            </ul>

<h3>Token Validation with JWKS</h3>
            <p>
                The <code>/jwks</code> endpoint exposes the API's public keys in the JWKS (JSON Web Key Set) format. 
                Any service can use these public keys to validate the signature of received JWT tokens, ensuring they were issued by this authentication API. 🔐
            </p>
            <p>
                To optimize performance, we use a cache that stores the public key for a few minutes, preventing excessive queries to the <code>/jwks</code> endpoint. 
                This reduces overhead and improves the scalability of the API. ⚡
            </p>

 <h3>Private Key Rotation</h3>
            <p>
                The private key used to sign tokens is securely stored in the database. 🔒 
                For security purposes, this key is automatically rotated after a few days, invalidating old tokens. 
                This process ensures that potential leaks are mitigated and that tokens always use the latest cryptography.
            </p>
                        <h3>Library Used</h3>
            <p>
                The entire JWKS implementation, including the generation and exposure of asymmetric keys, was developed using the custom library 
                <a href="https://github.com/Guidev123/EA-SharedLib" target="_blank">EA-SharedLib</a>. 
                This library simplifies the integration of authentication patterns based on JWT and JWKS, promoting code reuse and standardization across different projects. 📚
            </p>
        </section>
<section>
            <h2>Integration Flow with the Customer API</h2>
            <p>
                During the user creation process, the authentication API follows this flow:
            </p>
            <ol>
                <li>The user is created in the authentication API database. 🗂️</li>
                <li>An integration event is sent via RabbitMQ (RPC pattern) using a custom library. 🔗</li>
                <li>The Customer API processes the event and creates the customer in the Customer database. 🛠️</li>
                <li>
                    <strong>If the process succeeds:</strong> The authentication API generates a JWT and a refresh token for the user. ✅
                </li>
                <li>
                    <strong>If the process fails:</strong> The authentication API performs a rollback, deleting the created user and returning an error message. ❌
                </li>
            </ol>
        </section>

  <section>

<section>
    <h2>Endpoints</h2>

![image](https://github.com/user-attachments/assets/f2096c5c-b563-4e5b-998c-294575597b76)

    
</section>

</body>
