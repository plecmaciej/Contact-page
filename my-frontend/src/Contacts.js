import React, { useEffect, useState } from 'react';

// Contacts component to manage and display a list of contacts
const Contacts = ({ token }) => {
  const [contacts, setContacts] = useState([]);
  const [selectedContact, setSelectedContact] = useState(null);
  const [editableContact, setEditableContact] = useState(null);
  const [categories, setCategories] = useState([]);
  const [subcategories, setSubcategories] = useState([]);
  const [newContact, setNewContact] = useState(null);

  // Fetch contacts and categories when component mounts or token changes
  useEffect(() => {
    const fetchContacts = async () => {
      try {
        const response = await fetch('https://localhost:7094/api/contacts', {
          headers: token ? { Authorization: `Bearer ${token}` } : {},

        });
        if (response.ok) {
          const data = await response.json();
          console.log(data)
          setContacts(data);
        } else {
          console.error('Błąd przy pobieraniu kontaktów:', response.status);
        }
      } catch (error) {
        console.error('Fetch error:', error);
      }
    };

    const fetchCategories = async () => {
      try {
        const response = await fetch('https://localhost:7094/api/categories', {
          headers: token ? { Authorization: `Bearer ${token}` } : {},

        });
        if (response.ok) {
          const data = await response.json();
          setCategories(data);
        }
      } catch (error) {
        console.error('Błąd przy pobieraniu kategorii:', error);
      }
    };

    
      fetchContacts();
      fetchCategories();
    
  }, [token]);

  // Prepare contact for editing and fetch its subcategories
  const handleEditClick = () => {
    setEditableContact({ ...selectedContact });
    fetchSubcategories(selectedContact.categoryId);
  };

  // Fetch subcategories based on selected category
  const fetchSubcategories = async (categoryId) => {
    try {
      const response = await fetch(`https://localhost:7094/api/categories/${categoryId}/subcategories`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (response.ok) {
        const data = await response.json();
        setSubcategories(data);
      }
    } catch (error) {
      console.error('Blad subkategorii:', error);
    }
  };

  // Handle field changes during contact editing
  const handleChange = (field, value) => {
    const updated = { ...editableContact, [field]: value };
    if (field === 'categoryId') {
      updated.customSubcategory = null;
      updated.subcategoryId = null;
      fetchSubcategories(value);
    }
    setEditableContact(updated);
  };

  // Save the edited contact to the server
  const handleSave = async () => {
    try {
      const response = await fetch(`https://localhost:7094/api/contacts/${editableContact.id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(editableContact),
      });

      if (response.ok) {
        const updated = await response.json();
        setSelectedContact(updated);
        setContacts(contacts.map(c => (c.id === updated.id ? updated : c)));
        setEditableContact(null);
      } else {
        console.error('Błąd podczas zapisu:', response.status);
      }
    } catch (error) {
      console.error('Błąd sieci:', error);
    }
  };

  // Delete the selected contact
  const handleDelete = async () => {
    if (!selectedContact) return;
    if (!window.confirm('Czy na pewno chcesz usunąć ten kontakt?')) return;
  
    try {
      const response = await fetch(`https://localhost:7094/api/contacts/${selectedContact.id}`, {
        method: 'DELETE',
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
  
      if (response.ok) {
        setContacts(contacts.filter(c => c.id !== selectedContact.id));
        setSelectedContact(null);
        setEditableContact(null);
      } else {
        console.error('Błąd przy usuwaniu:', response.status);
      }
    } catch (err) {
      console.error('Błąd sieci przy usuwaniu:', err);
    }
  };

  // Initialize new contact creation
  const handleAddNew = () => {
    setNewContact({
      firstName: '',
      lastName: '',
      email: '',
      passwordHash: '',
      phoneNumber: '',
      birthDate: '',
      categoryId: '',
      subcategoryId: '',
      customSubcategory: ''
    });
  };
  
  // Create and save a new contact
  const handleCreate = async () => {
    if (!validatePassword(newContact.passwordHash)) {
      alert("Password must be at least 5 characters and contain at least one number.");
      return;
    }
  
    if (!isEmailUnique(newContact.email, contacts)) {
      alert("Email must be unique.");
      return;
    }
  
    try {
      const response = await fetch('https://localhost:7094/api/contacts', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(newContact),
      });
  
      if (response.ok) {
        const created = await response.json();
        setContacts([...contacts, created]);
        setNewContact(null);
      } else {
        console.error('Error creating contact:', response.status);
      }
    } catch (error) {
      console.error('Network error while creating contact:', error);
    }
  };

  // Validate password rules
  const validatePassword = (password) => {
    return password.length >= 5 && /\d/.test(password);
  };
  
  // Check if email is unique among contacts
  const isEmailUnique = (email, contacts, currentId = null) => {
    return !contacts.some(c => c.email === email && c.id !== currentId);
  };

  // Get the display name combining category and subcategory
  const getCategoryDisplayName = (contact) => {
    const cat = contact.categoryName || '';
    const sub = contact.subcategoryName || '';
    console.log(contact.category?.categoryName)
    return sub ? `${cat} - ${sub}` : cat;
  };

  
  // Render the component
  return (
    <div>
      <h2>Kontakty</h2>

      {/* Show "Add new contact" button only if logged in */}
      {token &&<button onClick={handleAddNew}>Dodaj nowy kontakt</button>}

       {/* Form to create a new contact */}
      {newContact && (
        <div style={{ marginTop: '20px', border: '1px solid #ccc', padding: '10px' }}>
          <h3>Nowy kontakt</h3>
          <label>Imię:
            <input value={newContact.firstName} onChange={(e) => setNewContact({ ...newContact, firstName: e.target.value })} />
          </label><br />
          <label>Nazwisko:
            <input value={newContact.lastName} onChange={(e) => setNewContact({ ...newContact, lastName: e.target.value })} />
          </label><br />
          <label>Email:
            <input value={newContact.email} onChange={(e) => setNewContact({ ...newContact, email: e.target.value })} />
          </label><br />
          <label>Hasło:
            <input value={newContact.passwordHash} onChange={(e) => setNewContact({ ...newContact, passwordHash: e.target.value })} />
          </label><br />
          <label>Telefon:
            <input value={newContact.phoneNumber} onChange={(e) => setNewContact({ ...newContact, phoneNumber: e.target.value })} />
          </label><br />
          <label>Data urodzenia:
            <input type="date" value={newContact.birthDate} onChange={(e) => setNewContact({ ...newContact, birthDate: e.target.value })} />
          </label><br />
          <label>Kategoria:
            <select
              value={newContact.categoryId}
              onChange={(e) => {
                const selectedId = parseInt(e.target.value);
                const selectedCategory = categories.find(c => c.id === selectedId);
                const isInne = selectedCategory?.name === 'Inne';
                const isSluzbowy = selectedCategory?.name === 'Służbowy';

                const updatedContact = {
                  ...newContact,
                  categoryId: selectedId,
                };

                // dealing with subcategoryid 
                if (isInne) {
                  updatedContact.customSubcategory = '';
                  delete updatedContact.subcategoryId;
                } else if (isSluzbowy) {
                  updatedContact.subcategoryId = '';
                  delete updatedContact.customSubcategory;
                } else {
                  delete updatedContact.customSubcategory;
                  delete updatedContact.subcategoryId;
                }

                setNewContact(updatedContact);
              }}
            >
              <option value="">-- Wybierz kategorię --</option>
              {categories.map((cat) => (
                <option key={cat.id} value={cat.id}>{cat.name}</option>
              ))}
            </select>
          </label><br />

            {/* Own category if "Inne" */}
            {categories.find(c => c.id === newContact.categoryId)?.name === 'Inne' && (
              <label>Własna podkategoria:
                <input
                  value={newContact.customSubcategory || ''}
                  onChange={(e) => setNewContact({ ...newContact, customSubcategory: e.target.value })}
                />
              </label>
            )}

            {/* choice of category if "Służbowy" */}
            {categories.find(c => c.id === newContact.categoryId)?.name === 'Służbowy' && (
              <label>Podkategoria:
                <select
                  value={newContact.subcategoryId || ''}
                  onChange={(e) => setNewContact({ ...newContact, subcategoryId: parseInt(e.target.value) })}
                >
                  <option value="">-- Wybierz podkategorię --</option>
                  {subcategories.map((sub) => (
                    <option key={sub.id} value={sub.id}>{sub.name}</option>
                  ))}
                </select>
              </label>
            )}
          <br /><br />
          {token && <button onClick={handleCreate}>Zapisz nowy kontakt</button>}
        </div>
      )}

      {/* if there is no existing contants or we want to edit contact */}
      <ul>
        {contacts.length === 0 ? (
          <p>Brak kontaktów do wyświetlenia</p>
        ) : (
          contacts.map((contact) => (
            <li
              key={contact.id}
              onClick={() => {
                setSelectedContact(contact);
                setEditableContact(null);
              }}
              style={{ cursor: 'pointer' }}
            >
              {contact.firstName} {contact.lastName} - {contact.email}
            </li>
          ))
        )}
      </ul>

      {selectedContact && (
        <div style={{ marginTop: '20px', borderTop: '1px solid #ccc', paddingTop: '10px' }}>
          <h3>Szczegóły kontaktu</h3>

          {/* contact attributes panel */}
          {!editableContact ? (
            <div>
              <p><strong>Imię:</strong> {selectedContact.firstName}</p>
              <p><strong>Nazwisko:</strong> {selectedContact.lastName}</p>
              <p><strong>Email:</strong> {selectedContact.email}</p>
              <p><strong>Haslo:</strong> {selectedContact.passwordHash}</p>
              <p><strong>Telefon:</strong> {selectedContact.phoneNumber}</p>
              <p><strong>Data urodzenia:</strong> {new Date(selectedContact.birthDate).toLocaleDateString()}</p>
              <p><strong>Kategoria:</strong> {getCategoryDisplayName(selectedContact)}</p>
              {token && (
                <>
                  <button onClick={handleEditClick}>Edytuj</button>
                  <button onClick={handleDelete} style={{ marginLeft: '10px', color: 'red' }}>Usuń</button>
                </>
              )}
            </div>
          ) : (
            <div>
              <label>
                Imię:
                <input
                  value={editableContact.firstName}
                  onChange={(e) => handleChange('firstName', e.target.value)}
                />
              </label><br />
              <label>
                Nazwisko:
                <input
                  value={editableContact.lastName}
                  onChange={(e) => handleChange('lastName', e.target.value)}
                />
              </label><br />
              <label>
                Email:
                <input
                  value={editableContact.email}
                  onChange={(e) => handleChange('email', e.target.value)}
                />
              </label><br />
              <label>
                Hasło:
                <input
                  value={editableContact.passwordHash}
                  onChange={(e) => handleChange('passwordHash', e.target.value)}
                />
              </label><br />
              <label>
                Telefon:
                <input
                  value={editableContact.phoneNumber}
                  onChange={(e) => handleChange('phoneNumber', e.target.value)}
                />
              </label><br />
              <label>
                Data urodzenia:
                <input
                  type="date"
                  value={editableContact.birthDate?.substring(0, 10)}
                  onChange={(e) => handleChange('birthDate', e.target.value)}
                />
              </label><br />
              <label>
                Kategoria:
                <select
                  value={editableContact.categoryId || ''}
                  onChange={(e) => handleChange('categoryId', parseInt(e.target.value))}
                >
                  <option value="">-- Wybierz kategorię --</option>
                  {categories.map((cat) => (
                    <option key={cat.id} value={cat.id}>{cat.name}</option>
                  ))}
                </select>
              </label><br />

              {categories.find(c => c.id === editableContact.categoryId)?.name === 'Inne' && (
                <label>
                  Własna podkategoria:
                  <input
                    value={editableContact.customSubcategory || ''}
                    onChange={(e) => handleChange('customSubcategory', e.target.value)}
                  />
                </label>
              )}

              {categories.find(c => c.id === editableContact.categoryId)?.name === 'Służbowy' && (
                <label>
                  Podkategoria:
                  <select
                    value={editableContact.subcategoryId || ''}
                    onChange={(e) => handleChange('subcategoryId', parseInt(e.target.value))}
                  >
                    <option value="">-- Wybierz podkategorię --</option>
                    {subcategories.map((sub) => (
                      <option key={sub.id} value={sub.id}>{sub.name}</option>
                    ))}
                  </select>
                </label>
              )}

              <br /><br />
              <button onClick={handleSave}>Zapisz zmiany</button>
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default Contacts;
